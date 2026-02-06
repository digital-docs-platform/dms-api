using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.DocumentVersion
{
    public sealed class EFGetDocumentVersionsByDocumentIdQuery : EFUseCase, IGetDocumentVersionsByDocumentIdQuery
    {
        public string RequiredPermission => PermissionCodes.DocumentVersionsRead;

        public PermissionScope Scope => PermissionScope.DocumentType;

        public int Id => 10;

        public string Name => "Get document versions by document id";

        public string Description => "Get document versions information by document identifier";
        public EFGetDocumentVersionsByDocumentIdQuery(DatabaseContext context)
            : base(context)
        {
            
        }

        public async Task<GetDocumentVersionsByDocumentIdResponse> ExecuteAsync(DocumentIdSearch search, CancellationToken ct)
        {

            var docInfo = await _context.Documents
                .AsNoTracking()
                .Where(d => d.Id == search.DocumentId && !d.IsDeleted)
                .Select(d => new { d.Id, d.DocumentTypeId })
                .FirstOrDefaultAsync(ct);

            if (docInfo is null)
                throw new EntityNotFoundException($"Document not found.");

            // 2) All field definitions for that docType
            var definitions = await _context.DocumentTypeFieldDefinitions
                .AsNoTracking()
                .Where(fd => fd.DocumentTypeId == docInfo.DocumentTypeId && !fd.IsDeleted)
                .OrderBy(fd => fd.SortOrder)
                .Select(fd => new
                {
                    fd.Id,
                    fd.Code,
                    fd.Label,
                    fd.Description,
                    fd.DataType,
                    fd.SortOrder
                })
                .ToListAsync(ct);

            // 3) Versions + FieldValues + OptionLabel (for Select)
            var versions = await _context.DocumentVersions
                .AsNoTracking()
                .Where(v => v.DocumentId == search.DocumentId && !v.IsDeleted)
                .OrderByDescending(v => v.VersionNumber)
                .Select(v => new
                {
                    v.Id,
                    v.VersionNumber,
                    v.CreatedAt,
                    v.IsCurrent,
                    v.CreatedByUser.FirstName,
                    v.CreatedByUser.LastName,
                    v.ChangeNote,
                    FieldValues = v.FieldValues
                        .Select(fv => new
                        {
                            fv.FieldDefinitionId,
                            fv.ValueString,
                            fv.ValueInt,
                            fv.ValueDecimal,
                            fv.ValueDate,
                            fv.ValueOptionId,
                            OptionLabel = fv.ValueOption != null && !fv.ValueOption.IsDeleted
                                ? fv.ValueOption.Label
                                : null
                        })
                        .ToList()
                })
                .ToListAsync(ct);

            // Helper: primitive -> JsonElement
            static JsonElement ToElement<T>(T value) => JsonSerializer.SerializeToElement(value);

            // 4) Map: for each version return ALL definitions + computed Value
            var versionDtos = versions.Select(v =>
            {
                var valuesByDefId = v.FieldValues.ToDictionary(x => x.FieldDefinitionId);

                var fields = definitions.Select(def =>
                {
                    valuesByDefId.TryGetValue(def.Id, out var fv);

                    JsonElement? valueEl = def.DataType switch
                    {
                        Domain.Enums.FieldDataType.Text
                            => fv?.ValueString is null ? null : ToElement(fv.ValueString),

                        Domain.Enums.FieldDataType.Number
                            => fv?.ValueInt is null ? null : ToElement(fv.ValueInt.Value),

                        Domain.Enums.FieldDataType.Decimal
                            => fv?.ValueDecimal is null ? null : ToElement(fv.ValueDecimal.Value),

                        Domain.Enums.FieldDataType.Date
                            => fv?.ValueDate is null ? null : ToElement(fv.ValueDate.Value),

                        Domain.Enums.FieldDataType.Select
                            => string.IsNullOrWhiteSpace(fv?.OptionLabel) ? null : ToElement(fv!.OptionLabel),

                        _ => fv?.ValueString is null ? null : ToElement(fv.ValueString)
                    };

                    return new GetDocumentVersionsByDocumentIdResponse.VersionFieldDto
                    {
                        FieldDefinitionId = def.Id,
                        Code = def.Code,
                        Label = def.Label,
                        Description = def.Description,
                        DataType = def.DataType,
                        Value = valueEl
                    };
                }).ToList();

                return new GetDocumentVersionsByDocumentIdResponse.DocumentVersionDto
                {
                    Id = v.Id,
                    VersionNumber = v.VersionNumber,
                    AddedBy = v.FirstName + " " + v.LastName,
                    Note = v.ChangeNote,
                    IsCurrent = v.IsCurrent,
                    CreatedAt = v.CreatedAt,
                    Fields = fields
                };
            }).ToList();

            return new GetDocumentVersionsByDocumentIdResponse
            {
                DocumentId = search.DocumentId,
                Versions = versionDtos
            };
        }
    }
}
