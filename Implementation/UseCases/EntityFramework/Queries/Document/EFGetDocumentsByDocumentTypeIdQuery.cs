using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Implementation.Querying.DocumentTypeFieldQuerying;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.Document
{
    public sealed class EFGetDocumentsByDocumentTypeIdQuery : EFUseCase, IGetDocumentsByDocumentTypeIdQuery
    {
        public string RequiredPermission => PermissionCodes.DocumentsRead;
        public PermissionScope Scope => PermissionScope.DocumentType;

        public int Id => 8;
        public string Name => "Get documents by document type";
        public string Description => "Returns current document versions with filter/sort/pagination.";

        private readonly IFieldQueryDispatcher _fieldQueryDispatcher;

        public EFGetDocumentsByDocumentTypeIdQuery(
            IFieldQueryDispatcher fieldQueryDispatcher,
            DatabaseContext context)
            : base(context)
        {
            _fieldQueryDispatcher = fieldQueryDispatcher;
        }

        public async Task<GetDocumentsByDocumentTypeIdResponse> ExecuteAsync(GetDocumentsByDocumentTypeIdSearch search, CancellationToken ct)
        {
            // Pagination normalization
            var pageIndex = search.Pagination?.PageIndex ?? 0;
            var pageSize = search.Pagination?.PageSize ?? 10;

            if (pageIndex < 0) pageIndex = 0;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            // Load DocumentType + FieldDefinitions
            var docType = await _context.DocumentTypes
                .Include(dt => dt.FieldDefinitions)
                .FirstOrDefaultAsync(dt => dt.Id == search.DocumentTypeId && !dt.IsDeleted, ct);

            if (docType is null)
                throw new EntityNotFoundException("Document type not found.");

            var defsByCode = docType.FieldDefinitions
                .Where(fd => !fd.IsDeleted)
                .ToDictionary(fd => fd.Code, StringComparer.OrdinalIgnoreCase);

            // Base query: current versions for this doc type
            IQueryable<DocumentVersion> query = _context.DocumentVersions
                .AsNoTracking()
                .Where(v => v.IsCurrent && !v.IsDeleted)
                .Where(v => v.DocumentInstance.DocumentTypeId == search.DocumentTypeId && !v.DocumentInstance.IsDeleted);

            // Filters
            if (search.Filters is not null)
            {
                foreach (var (code, json) in search.Filters)
                {
                    if (!defsByCode.TryGetValue(code, out var def)) continue;
                    if (!def.IsSearchable) continue;

                    query = _fieldQueryDispatcher.ApplyFilter(query, def, json);
                }
            }

            // Total count (after filters)
            var totalCount = await query.CountAsync(ct);

            // Sorting (dynamic fields)
            if (search.Sort != null &&
                !string.IsNullOrWhiteSpace(search.Sort.Active) &&
                defsByCode.TryGetValue(search.Sort.Active, out var sortDef))
            {
                var desc = string.Equals(search.Sort.Direction, "desc", StringComparison.OrdinalIgnoreCase);
                query = _fieldQueryDispatcher.ApplySort(query, sortDef, desc);
            }
            else
            {
                // IMPORTANT: default sort for stable paging
                query = query.OrderByDescending(v => v.CreatedAt);
            }

            // Page + includes for mapping
            var page = await query
                .Include(v => v.DocumentInstance)
                .Include(v => v.FieldValues)
                    .ThenInclude(fv => fv.FieldDefinition)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            // Map
            var items = page.Select(v =>
            {
                var dto = new DocumentListItemDto
                {
                    DocumentId = v.DocumentId,
                    Title = v.DocumentInstance.Title,
                    DocumentTypeId = v.DocumentInstance.DocumentTypeId,
                    CurrentVersionNumber = v.VersionNumber,
                    VersionCreatedAt = v.CreatedAt
                };

                foreach (var fv in v.FieldValues)
                {
                    var code = fv.FieldDefinition?.Code;
                    if (string.IsNullOrWhiteSpace(code))
                        continue;

                    dto.Fields[code] = fv.FieldDefinition!.DataType switch
                    {
                        FieldDataType.Text => fv.ValueString,
                        FieldDataType.Number => fv.ValueInt,
                        FieldDataType.Decimal => fv.ValueDecimal,
                        FieldDataType.Date => fv.ValueDate,
                        FieldDataType.Select => fv.ValueOptionId,
                        _ => fv.ValueString
                    };
                }

                return dto;
            }).ToList();

            return new GetDocumentsByDocumentTypeIdResponse
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
