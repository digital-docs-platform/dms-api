using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying.Strategies
{
    public sealed class DecimalFieldQueryStrategy : IFieldQueryStrategy
    {
        public FieldDataType DataType => FieldDataType.Decimal;

        public bool CanSort => true;

        public IQueryable<DocumentVersion> ApplyFilter(IQueryable<DocumentVersion> query, int fieldDefinitionId, JsonElement filter)
        {
            var op = FilterJson.GetOp(filter);
            if (string.IsNullOrWhiteSpace(op))
                return query;

            return op switch
            {
                FilterOperations.Eq => ApplyEq(query, fieldDefinitionId, filter),
                FilterOperations.Gt => ApplyCompare(query, fieldDefinitionId, filter, ">"),
                FilterOperations.Gte => ApplyCompare(query, fieldDefinitionId, filter, ">="),
                FilterOperations.Lt => ApplyCompare(query, fieldDefinitionId, filter, "<"),
                FilterOperations.Lte => ApplyCompare(query, fieldDefinitionId, filter, "<="),
                FilterOperations.Between => ApplyBetween(query, fieldDefinitionId, filter),
                _ => query
            };
        }

        public IQueryable<DocumentVersion> ApplySort(IQueryable<DocumentVersion> query, int fieldDefinitionId, bool desc)
        {
            return desc
           ? query.OrderByDescending(v => v.FieldValues
               .Where(fv => fv.FieldDefinitionId == fieldDefinitionId)
               .Select(fv => fv.ValueDecimal)
               .FirstOrDefault())
           : query.OrderBy(v => v.FieldValues
               .Where(fv => fv.FieldDefinitionId == fieldDefinitionId)
               .Select(fv => fv.ValueDecimal)
               .FirstOrDefault());
        }

        private static IQueryable<DocumentVersion> ApplyEq(
            IQueryable<DocumentVersion> query,
            int defId,
            JsonElement filter)
        {
            if (!FilterJson.TryGetDecimalValue(filter, out var value))
                return query;

            return query.Where(v => v.FieldValues.Any(fv =>
                fv.FieldDefinitionId == defId &&
                fv.ValueDecimal == value));
        }

        private static IQueryable<DocumentVersion> ApplyCompare(
         IQueryable<DocumentVersion> query,
         int defId,
         JsonElement filter,
         string op)
        {
            if (!FilterJson.TryGetDecimalValue(filter, out var value))
                return query;

            return op switch
            {
                ">" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDecimal > value)),
                ">=" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDecimal >= value)),
                "<" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDecimal < value)),
                "<=" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDecimal <= value)),
                _ => query
            };
        }

        private static IQueryable<DocumentVersion> ApplyBetween(
            IQueryable<DocumentVersion> query,
            int defId,
            JsonElement filter)
        {
            var min = FilterJson.GetDecimal(filter, "min");
            var max = FilterJson.GetDecimal(filter, "max");

            if (!min.HasValue && !max.HasValue)
                return query;

            if (min.HasValue)
                query = query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDecimal >= min.Value));

            if (max.HasValue)
                query = query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDecimal <= max.Value));

            return query;
        }

    }
}
