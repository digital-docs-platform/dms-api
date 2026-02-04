using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying.Strategies
{
    public sealed class TextFieldQueryStrategy : IFieldQueryStrategy
    {
        public FieldDataType DataType => FieldDataType.Text;

        public bool CanSort => true;

        public IQueryable<DocumentVersion> ApplyFilter(IQueryable<DocumentVersion> query, int fieldDefinitionId, JsonElement filter)
        {
            var op = FilterJson.GetOp(filter) ?? FilterOperations.Contains;
            if (!string.Equals(op, FilterOperations.Contains, StringComparison.OrdinalIgnoreCase))
                return query;

            var value = FilterJson.GetStringValue(filter);
            if (string.IsNullOrWhiteSpace(value))
                return query;

            value = value.Trim();

            return query.Where(v => v.FieldValues.Any(fv =>
                fv.FieldDefinitionId == fieldDefinitionId &&
                fv.ValueString != null &&
                EF.Functions.Like(fv.ValueString, $"%{EscapeLike(value)}%")));

        }

        public IQueryable<DocumentVersion> ApplySort(IQueryable<DocumentVersion> query, int fieldDefinitionId, bool desc)
        {

            return desc
                ? query.OrderByDescending(v => v.FieldValues
                    .Where(fv => fv.FieldDefinitionId == fieldDefinitionId)
                    .Select(fv => fv.ValueString)
                    .FirstOrDefault())
                : query.OrderBy(v => v.FieldValues
                    .Where(fv => fv.FieldDefinitionId == fieldDefinitionId)
                    .Select(fv => fv.ValueString)
                    .FirstOrDefault());
        }

        private static string EscapeLike(string input)
       => input.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
    }
}
