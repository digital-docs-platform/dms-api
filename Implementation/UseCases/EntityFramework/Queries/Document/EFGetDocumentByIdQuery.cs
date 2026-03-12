using Application.Exceptions;
using Application.PermissionHandling;
using Application.Storage;
using Application.UseCases;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.Document
{
    public sealed class EFGetDocumentByIdQuery : EFUseCase, IGetDocumentByIdQuery
    {
        public string RequiredPermission => PermissionCodes.DocumentsRead;

        public PermissionScope Scope => PermissionScope.Document;

        public int Id => 9;

        public string Name => "Get document by id";

        public string Description => "Get document information by unique identifier";

        private readonly IFileStorageService _fileStorageService;
        public EFGetDocumentByIdQuery(IFileStorageService fileStorageService, DatabaseContext context)
            : base(context)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<GetDocumentByIdResponse> ExecuteAsync(DocumentIdSearch search, CancellationToken ct)
        {
            var document = await _context.Documents
                                        .AsNoTracking()
                                        .Include(d => d.CreatedByUser)
                                        .FirstOrDefaultAsync(d => d.Id == search.DocumentId, ct);

            if (document == null)
                throw new EntityNotFoundException("Document not found!");

            var docType = await _context.DocumentTypes
               .AsNoTracking()
               .FirstOrDefaultAsync(dt => dt.Id == document.DocumentTypeId, ct);

            if (docType is null)
                throw new EntityNotFoundException("Document type not found for this document.");

            var latest = await _context.DocumentVersions
                .AsNoTracking()
                .Where(v => v.DocumentId == document.Id)
                .Include(d => d.CreatedByUser)
                .Include(v => v.Files)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if(latest is null)
                throw new EntityNotFoundException("Latest version not found for this document.");

            var response = new GetDocumentByIdResponse
            {
                Id = document.Id,
                Title = document.Title,
                DocumentTypeId = document.DocumentTypeId,
                DocumentTypeName = docType.Name,
                CreatedById = document.CreatedBy,
                CreatedByName  = document.CreatedByUser.FirstName + " " + document.CreatedByUser.LastName,
                CreatedAt = document.CreatedAt,
                ModifiedAt = document.ModifiedAt,
                LatestVersion = null
                
            };

            var values = await _context.DocumentTypeFieldValues
                .AsNoTracking()
                .Where(fv => fv.VersionId == latest.Id)
                .Include(fv => fv.FieldDefinition)
                .Include(fv => fv.ValueOption)
                .ToListAsync(ct);

            response.LatestVersion = new LatestDocumentVersionDto
            {
                Id = latest.Id,
                VersionNumber = latest.VersionNumber,
                CreatedById = latest.CreatedBy,
                CreatedByName = latest.CreatedByUser.FirstName + " " + latest.CreatedByUser.LastName,
                CreatedAt = latest.CreatedAt,
                Fields = values
                   .Select(MapFieldValue)
                   .OrderBy(x => x.FieldDefinitionId)
                   .ToList()
            };

            foreach (var file in latest.Files.OrderBy(f => f.Order))
            {
                var url = await _fileStorageService.GetPresignedUrlAsync(file.StorageKey, expirySeconds: 3600, ct);
                response.Files.Add(new DocumentFileResponse
                {
                    Id = file.Id,
                    Name = file.Name,
                    Extension = file.Extension.ToString(),
                    ContentType = file.ContentType,
                    SizeInBytes = file.SizeInBytes,
                    Order = file.Order,
                    PresignedUrl = url
                });
            }


            return response;
        }




        private static DocumentFieldValueDto MapFieldValue(DocumentTypeFieldValue fv)
        {
            var fd = fv.FieldDefinition;

            object? value = null;

            switch (fd.DataType)
            {
                case FieldDataType.Text:
                    value = fv.ValueString;
                    break;

                case FieldDataType.Number:
                    value = fv.ValueInt;
                    break;

                case FieldDataType.Decimal:
                    value = fv.ValueDecimal;
                    break;

                case FieldDataType.Date:
                    value = fv.ValueDate;
                    break;

                case FieldDataType.Select:

                    if (!string.IsNullOrWhiteSpace(fv.ValueOption?.Label))
                        value = fv.ValueOption.Label;
                    else
                        value = fv.ValueOptionId; // int?
                    break;

                default:
                    value = fv.ValueString;
                    break;
            }


            return new DocumentFieldValueDto
            {
                FieldDefinitionId = fv.FieldDefinitionId,
                Code = fd.Code,
                Label = fd.Label,
                DataType = fd.DataType,

                Value = value,

                OptionId = fv.ValueOptionId,
                OptionLabel = fv.ValueOption?.Label
            };
        }




    }
}
