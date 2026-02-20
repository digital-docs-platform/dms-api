using Application.Exceptions;
using Application.Listings;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Search;
using DataAccess;
using Domain.Enums;
using Implementation.Querying.DocumentTypeFieldQuerying;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DocumentVersionEntity = Domain.Entities.DocumentVersion;

namespace Implementation.UseCases.EntityFramework.Queries.Listings
{
    public sealed class EFExportDocumentsListingQuery : EFUseCase, IExportDocumentsListingQuery
    {
        public string RequiredPermission => PermissionCodes.DocumentsExport;
        public PermissionScope Scope => PermissionScope.DocumentType;

        public int Id => 11;
        public string Name => "Export documents";
        public string Description => "Export all documents filtered by user in chosen format.";

        private const int MaxExportRows = 5000;

        private readonly IFieldQueryDispatcher _fieldQueryDispatcher;
        private readonly IListingExportDispatcher _listingExportDispatcher;

        public EFExportDocumentsListingQuery(
            IFieldQueryDispatcher fieldQueryDispatcher,
            IListingExportDispatcher listingExportDispatcher,
            DatabaseContext context)
            : base(context)
        {
            _fieldQueryDispatcher = fieldQueryDispatcher;
            _listingExportDispatcher = listingExportDispatcher;
        }

        public async Task<ExportFileResult> ExecuteAsync(ExportDocumentsListingSearch search, CancellationToken ct)
        {
            
            var docType = await _context.DocumentTypes
                .Include(dt => dt.FieldDefinitions)
                .FirstOrDefaultAsync(dt => dt.Id == search.DocumentTypeId && !dt.IsDeleted, ct);

            if (docType is null)
                throw new EntityNotFoundException("Document type not found.");

            var defs = docType.FieldDefinitions.Where(fd => !fd.IsDeleted).ToList();

            var defsByCode = defs
                .ToDictionary(fd => fd.Code, StringComparer.OrdinalIgnoreCase);

           
            var errors = new List<ValidationError>();

            foreach (var col in search.Columns)
            {
                if (!defsByCode.ContainsKey(col))
                    errors.Add(new ValidationError("columns", $"Unknown column '{col}' for this document type."));
            }

            if (errors.Count > 0)
                throw new RequestDataValidationException(errors);

            // Preload SELECT options labels (id -> label)
            // (samo za select definicije ovog docType-a)
            var selectDefIds = defs
                .Where(d => d.DataType == FieldDataType.Select)
                .Select(d => d.Id)
                .ToList();

            Dictionary<int, string> optionLabelsById = new();

            if (selectDefIds.Count > 0)
            {
                optionLabelsById = await _context.DocumentTypeFieldOptions
                    .AsNoTracking()
                    .Where(o => !o.IsDeleted && selectDefIds.Contains(o.FieldDefinitionId))
                    .ToDictionaryAsync(o => o.Id, o => o.Label, ct);
            }

            
            IQueryable<DocumentVersionEntity> query = _context.DocumentVersions
                .AsNoTracking()
                .Where(v => v.IsCurrent && !v.IsDeleted)
                .Where(v => v.DocumentInstance.DocumentTypeId == search.DocumentTypeId && !v.DocumentInstance.IsDeleted);

           
            if (search.Filters is not null)
            {
                foreach (var (code, json) in search.Filters)
                {
                    if (!defsByCode.TryGetValue(code, out var def)) continue;
                    if (!def.IsSearchable) continue;

                    query = _fieldQueryDispatcher.ApplyFilter(query, def, json);
                }
            }

            
            if (search.Sort != null &&
                !string.IsNullOrWhiteSpace(search.Sort.Active) &&
                defsByCode.TryGetValue(search.Sort.Active, out var sortDef))
            {
                var desc = string.Equals(search.Sort.Direction, "desc", StringComparison.OrdinalIgnoreCase);
                query = _fieldQueryDispatcher.ApplySort(query, sortDef, desc);
            }
            else
            {
                query = query.OrderByDescending(v => v.CreatedAt);
            }

          
            var result = await query
                .Include(v => v.FieldValues)
                    .ThenInclude(fv => fv.FieldDefinition)
                .Take(MaxExportRows)
                .ToListAsync(ct);

           
            var headers = search.Columns
                .Select(c => defsByCode[c].Label)
                .ToList();

          
            var rows = new List<List<string>>(result.Count);

            foreach (var v in result)
            {
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var fv in v.FieldValues)
                {
                    var def = fv.FieldDefinition;
                    if (def is null || def.IsDeleted) continue;

                    map[def.Code] = def.DataType switch
                    {
                        FieldDataType.Text => fv.ValueString ?? "",
                        FieldDataType.Number => fv.ValueInt?.ToString(CultureInfo.InvariantCulture) ?? "",
                        FieldDataType.Decimal => fv.ValueDecimal?.ToString(CultureInfo.InvariantCulture) ?? "",
                        FieldDataType.Date => fv.ValueDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",

                        //label umesto optionId
                        FieldDataType.Select =>
                            fv.ValueOptionId.HasValue && optionLabelsById.TryGetValue(fv.ValueOptionId.Value, out var lbl)
                                ? lbl
                                : "",

                        _ => fv.ValueString ?? ""
                    };
                }

                var row = new List<string>(search.Columns.Count);
                foreach (var col in search.Columns)
                    row.Add(map.TryGetValue(col, out var val) ? val : "");

                rows.Add(row);
            }

            
            var listingDoc = new ListingDocument
            {
                Title = search.Title,
                Headers = headers,
                Rows = rows
            };

            return await _listingExportDispatcher.ExecuteAsync(search.Format, listingDoc, ct);
        }
    }
}
