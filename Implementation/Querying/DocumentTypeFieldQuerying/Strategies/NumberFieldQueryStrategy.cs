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
    public sealed class NumberFieldQueryStrategy : IFieldQueryStrategy
    {
        public FieldDataType DataType => FieldDataType.Number;
        public bool CanSort => true;

        public IQueryable<DocumentVersion> ApplyFilter(
            IQueryable<DocumentVersion> query,
            int fieldDefinitionId,
            JsonElement filter)
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

        public IQueryable<DocumentVersion> ApplySort(
            IQueryable<DocumentVersion> query,
            int fieldDefinitionId,
            bool desc)
        {
            return desc
                ? query.OrderByDescending(v => v.FieldValues
                    .Where(fv => fv.FieldDefinitionId == fieldDefinitionId)
                    .Select(fv => fv.ValueInt)
                    .FirstOrDefault())
                : query.OrderBy(v => v.FieldValues
                    .Where(fv => fv.FieldDefinitionId == fieldDefinitionId)
                    .Select(fv => fv.ValueInt)
                    .FirstOrDefault());
        }

        private static IQueryable<DocumentVersion> ApplyEq(IQueryable<DocumentVersion> query, int defId, JsonElement filter)
        {
            if (!FilterJson.TryGetIntValue(filter, out var value))
                return query;

            return query.Where(v => v.FieldValues.Any(fv =>
                fv.FieldDefinitionId == defId &&
                fv.ValueInt == value));
        }

        private static IQueryable<DocumentVersion> ApplyCompare(
            IQueryable<DocumentVersion> query,
            int defId,
            JsonElement filter,
            string op)
        {
            if (!FilterJson.TryGetIntValue(filter, out var value))
                return query;

            return op switch
            {
                ">" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueInt > value)),
                ">=" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueInt >= value)),
                "<" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueInt < value)),
                "<=" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueInt <= value)),
                _ => query
            };
        }

        private static IQueryable<DocumentVersion> ApplyBetween(IQueryable<DocumentVersion> query, int defId, JsonElement filter)
        {
            var min = FilterJson.GetInt(filter, "min");
            var max = FilterJson.GetInt(filter, "max");

            if (!min.HasValue && !max.HasValue)
                return query;

            if (min.HasValue)
                query = query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueInt >= min.Value));

            if (max.HasValue)
                query = query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueInt <= max.Value));

            return query;
        }
    }
}
