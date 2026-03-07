using Application;
using Application.DocumentFields;
using Application.Exceptions;
using Application.Logging;
using Application.PermissionHandling;
using Application.Storage;
using Application.UseCases;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Document;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Implementation.UseCases.EntityFramework.Commands.Document
{
    public sealed class EFCreateDocumentCommand : EFUseCase, ICreateDocumentCommand
    {
        public string RequiredPermission => PermissionCodes.DocumentsWrite;
        public PermissionScope Scope => PermissionScope.Document;

        public int Id => 4;
        public string Name => "Create new document";
        public string Description => "Create new instance that represents a document (not new DocumentVersion)";

        public AuditLogEntry BuildAuditEntry(CreateDocumentRequest input, IApplicationActor actor)
        {
            return new AuditLogEntry
            {
                ActorEmail = actor.Email,
                ActorId = actor.Id,
                EntityId = input.Id,
                EntityType = nameof(Domain.Entities.Document),
                EntityName = input.Title,
                EventType = AuditEventType.DocumentCreated,
                Metadata = new
                {
                    Title = input.Title,
                    DocumentTypeId = input.DocumentTypeId,
                    FieldsInput = input.FieldsInput.Select(f => new
                    {
                        f.FieldDefinitionId,
                        f.Value
                    })
                },
                IpAddress = null,
            };
        }

        private readonly IApplicationActor _actor;
        private readonly IFieldValueMapper _fieldValueMapper;
        private readonly IFileStorageService _fileStorageService;

        public EFCreateDocumentCommand(
            DatabaseContext context,
            IApplicationActor actor,
            IFieldValueMapper fieldValueMapper,
            IFileStorageService fileStorageService) : base(context)
        {
            _actor = actor;
            _fieldValueMapper = fieldValueMapper;
            _fileStorageService = fileStorageService;
        }

        public async Task ExecuteAsync(CreateDocumentRequest request, CancellationToken ct)
        {
            var errors = new List<ValidationError>();

            // --- Document type validation ---

            var docTypeExists = await _context.DocumentTypes
                .AnyAsync(dt => dt.Id == request.DocumentTypeId && dt.IsDeleted == false, ct);

            if (!docTypeExists)
            {
                errors.Add(new ValidationError(nameof(request.DocumentTypeId), "DocumentType not found."));
                throw new RequestDataValidationException(errors);
            }

            var defs = await _context.DocumentTypeFieldDefinitions
                .Where(d => d.DocumentTypeId == request.DocumentTypeId && d.IsDeleted == false)
                .ToListAsync(ct);

            var defsById = defs.ToDictionary(d => d.Id);

            foreach (var input in request.FieldsInput)
            {
                if (!defsById.ContainsKey(input.FieldDefinitionId))
                {
                    errors.Add(new ValidationError(
                        property: $"fieldsInput[{input.FieldDefinitionId}]",
                        message: "Unknown fieldDefinitionId for given documentType."
                    ));
                }
            }

            foreach (var def in defs.Where(d => d.IsRequired))
            {
                var inp = request.FieldsInput.FirstOrDefault(x => x.FieldDefinitionId == def.Id);

                if (inp == null)
                {
                    errors.Add(new ValidationError(def.Code ?? def.Id.ToString(), "Required field missing."));
                    continue;
                }

                if (IsEmptyJson(inp.Value))
                    errors.Add(new ValidationError(def.Code ?? def.Id.ToString(), "Required field value is empty."));
            }

            // --- File validation ---

            if (!request.Files.Any())
                errors.Add(new ValidationError(nameof(request.Files), "At least one file is required."));

            const long maxFileSizeBytes = 20 * 1024 * 1024; // 20MB

            var validatedFiles = new List<(DocumentFileInput Input, FileExtension Extension)>();

            foreach (var file in request.Files)
            {
                var rawExt = Path.GetExtension(file.FileName).TrimStart('.');

                if (!Enum.TryParse<FileExtension>(rawExt, ignoreCase: true, out var fileExtension))
                {
                    errors.Add(new ValidationError(nameof(request.Files),
                        $"File '{file.FileName}' has unsupported extension '.{rawExt}'."));
                    continue;
                }

                if (file.SizeInBytes > maxFileSizeBytes)
                {
                    errors.Add(new ValidationError(nameof(request.Files),
                        $"File '{file.FileName}' exceeds the maximum allowed size of 20MB."));
                    continue;
                }

                validatedFiles.Add((file, fileExtension));
            }

            if (errors.Count > 0)
                throw new RequestDataValidationException(errors);

            // --- Build version ---

            var versionId = Guid.NewGuid();

            var version = new DocumentVersion
            {
                Id = versionId,
                VersionNumber = 1,
                CreatedBy = _actor.Id,
                IsCurrent = true,
                ChangeNote = "initial insert",
                FieldValues = new List<DocumentTypeFieldValue>(),
                Files = new List<DocumentVersionFile>()
            };

            // --- Field value mapping ---

            foreach (var input in request.FieldsInput)
            {
                if (!defsById.TryGetValue(input.FieldDefinitionId, out var def))
                    continue;

                if (!def.IsRequired && IsEmptyJson(input.Value))
                    continue;

                if (def.DataType == FieldDataType.Select)
                {
                    if (input.Value.ValueKind != JsonValueKind.Number || !input.Value.TryGetInt32(out var optionId))
                    {
                        errors.Add(new ValidationError(def.Code ?? def.Id.ToString(), "Select field expects optionId (number)."));
                        continue;
                    }

                    var optionExists = await _context.DocumentTypeFieldOptions
                        .AnyAsync(o => o.Id == optionId && o.FieldDefinitionId == def.Id && o.IsDeleted == false, ct);

                    if (!optionExists)
                    {
                        errors.Add(new ValidationError(def.Code ?? def.Id.ToString(), "Invalid optionId for this field."));
                        continue;
                    }
                }

                var fv = new DocumentTypeFieldValue { FieldDefinitionId = def.Id };

                if (!_fieldValueMapper.TryApply(def.DataType, input.Value, fv, out var errorMessage))
                {
                    errors.Add(new ValidationError(def.Code ?? def.Id.ToString(), errorMessage));
                    continue;
                }

                version.FieldValues.Add(fv);
            }

            if (errors.Count > 0)
                throw new RequestDataValidationException(errors);

            // --- Upload files to MinIO, then build DocumentVersionFile entities ---

            foreach (var (fileInput, fileExtension) in validatedFiles)
            {
                var fileId = Guid.NewGuid();
                var uniqueFileName = $"{fileId}_{fileInput.FileName}";
                var storageKey = $"{request.Id}/{versionId}/{uniqueFileName}";

                await _fileStorageService.UploadAsync(
                    storageKey,
                    fileInput.Content,
                    fileInput.SizeInBytes,
                    fileInput.ContentType,
                    ct);

                version.Files.Add(new DocumentVersionFile
                {
                    Id = fileId,
                    DocumentVersionId = versionId,
                    Name = uniqueFileName,
                    Extension = fileExtension,
                    ContentType = fileInput.ContentType,
                    SizeInBytes = fileInput.SizeInBytes,
                    StorageKey = storageKey,
                    Order = fileInput.Order
                });
            }

            // --- Persist ---

            var doc = new Domain.Entities.Document
            {
                Id = request.Id,
                DocumentTypeId = request.DocumentTypeId,
                Title = (request.Title ?? string.Empty).Trim(),
                CreatedBy = _actor.Id,
                DocumentVersions = new List<DocumentVersion> { version }
            };

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync(ct);
        }

        private static bool IsEmptyJson(JsonElement el)
        {
            if (el.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                return true;

            if (el.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(el.GetString()))
                return true;

            return false;
        }
    }
}
