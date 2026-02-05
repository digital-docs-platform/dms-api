using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Implementation.Querying.DocumentTypeFieldQuerying;

namespace Implementation.Querying.FieldQuerying.Strategies;

public sealed class SelectFieldQueryStrategy : IFieldQueryStrategy
{
    public FieldDataType DataType => FieldDataType.Select;
    public bool CanSort => false;

    public IQueryable<DocumentVersion> ApplyFilter(IQueryable<DocumentVersion> query, int fieldDefinitionId, JsonElement filter)
    {
        var op = FilterJson.GetOp(filter);
        if (string.IsNullOrWhiteSpace(op))
            return query;

        return op switch
        {
            FilterOperations.Eq => ApplyEq(query, fieldDefinitionId, filter),
            FilterOperations.Any => ApplyAny(query, fieldDefinitionId, filter),
            _ => query
        };
    }

    public IQueryable<DocumentVersion> ApplySort(IQueryable<DocumentVersion> query, int fieldDefinitionId, bool desc)
        => query;

    private static IQueryable<DocumentVersion> ApplyEq(IQueryable<DocumentVersion> query, int defId, JsonElement filter)
    {
        if (!FilterJson.TryGetIntValue(filter, out var optionId))
            return query;

        return query.Where(v => v.FieldValues.Any(fv =>
            fv.FieldDefinitionId == defId &&
            fv.ValueOptionId == optionId));
    }

    private static IQueryable<DocumentVersion> ApplyAny(IQueryable<DocumentVersion> query, int defId, JsonElement filter)
    {
        var optionIds = FilterJson.GetIntArray(filter, "values");
        if (optionIds.Count == 0)
            return query;

        return query.Where(v => v.FieldValues.Any(fv =>
            fv.FieldDefinitionId == defId &&
            fv.ValueOptionId.HasValue &&
            optionIds.Contains(fv.ValueOptionId.Value)));
    }
}
