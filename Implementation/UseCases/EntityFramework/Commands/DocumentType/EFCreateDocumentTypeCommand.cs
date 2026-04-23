using Application;
using Application.Exceptions;
using Application.Logging;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.DocumentType;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Implementation.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Implementation.UseCases.EntityFramework.Commands.DocumentType
{
    public sealed class EFCreateDocumentTypeCommand : EFUseCase, ICreateDocumentTypeCommand
    {
        public string RequiredPermission => PermissionCodes.SystemAdmin;
        public PermissionScope Scope => PermissionScope.System;

        public int Id => 3;
        public string Name => "Create DocumentType";
        public string Description => "Create new Document Type";

        private readonly IApplicationActor _actor;

        public AuditLogEntry BuildAuditEntry(CreateDocumentTypeRequest input, IApplicationActor actor)
        {
            return new AuditLogEntry
            {
                ActorEmail = actor.Email,
                ActorId = actor.Id,
                EntityId = input.DocumentTypeId,
                EntityType = nameof(Domain.Entities.DocumentType),
                EntityName = input.Name,
                EventType = AuditEventType.DocumentTypeCreated,
                Metadata = new
                {
                    input.Name,
                    input.Description,
                    FieldDefinitions = input.FieldDefinitions.Select(f => new
                    {
                        f.Label,
                        f.Description,
                        f.DataType,
                        f.IsRequired,
                        f.IsSearchable,
                        f.IsSortable,
                        Options = f.DataType != FieldDataType.Select ? null : f.SelectOptions?.Select(o => new
                        {
                            o.Label,
                            o.SortOrder
                        }).ToList()
                    }).ToList()
                }
            };
        }


        public EFCreateDocumentTypeCommand(IApplicationActor actor, DatabaseContext context) : base(context)
        {
            _actor = actor;
        }

        public async Task ExecuteAsync(CreateDocumentTypeRequest request, CancellationToken ct)
        {
            var exists = await _context.DocumentTypes
                .AnyAsync(x => x.DeletedAt == null && x.Name.ToLower() == request.Name.ToLower(), ct);

            if (exists)
                throw new EntityAlreadyExistsException("Document type with this name already exists.");

            var docType = new Domain.Entities.DocumentType
            {
                Id = request.DocumentTypeId,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                AddedBy = _actor.Id,
                FieldDefinitions = request.FieldDefinitions.Select(MapFieldDefinition).ToList()
            };

            await _context.DocumentTypes.AddAsync(docType, ct);
            await _context.SaveChangesAsync(ct);
        }

        private static DocumentTypeFieldDefinition MapFieldDefinition(CreateDocumentTypeFieldRequest f)
        {
            var def = new DocumentTypeFieldDefinition
            {
                Label = f.Label.Trim(),
                Description = f.Description.Trim(),
                DataType = f.DataType,
                SortOrder = f.SortOrder,
                IsRequired = f.IsRequired,
                IsSearchable = f.IsSearchable,
                IsSortable = f.IsSortable,
                Code = f.Label.Slugify()
            };

            // SELECT: options
            if (f.DataType == FieldDataType.Select)
            {
                var options = f.SelectOptions ?? new List<CreateDocumentTypeFieldOptionRequest>();

                if (options.Count == 0)
                    throw new RequestDataValidationException(new List<ValidationError>
                {
                    new("options", "Select field must have at least one option.")
                });

                var dup = options
                    .Select(o => o.Label.Trim().Slugify())
                    .GroupBy(v => v, StringComparer.OrdinalIgnoreCase)
                    .Any(g => g.Count() > 1);

                if (dup)
                    throw new RequestDataValidationException(new List<ValidationError>
                {
                    new("options", "Duplicate option values are not allowed for the same field.")
                });


                def.Options = options
                    .Where(o => !string.IsNullOrWhiteSpace(o.Label))
                    .Select(o => new DocumentTypeFieldOption
                    {
                        Value = o.Label.Trim().Slugify(),
                        Label = o.Label.Trim(),
                        SortOrder = o.SortOrder
                    })
                    .OrderBy(o => o.SortOrder)
                    .ToList();
            }

            return def;
        }

       
    }
}
