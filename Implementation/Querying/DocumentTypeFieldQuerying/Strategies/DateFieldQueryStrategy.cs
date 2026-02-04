using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying.Strategies
{
    public sealed class DateFieldQueryStrategy : IFieldQueryStrategy
    {
        public FieldDataType DataType => FieldDataType.Date;
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
                    .Select(fv => fv.ValueDate)
                    .FirstOrDefault())
                : query.OrderBy(v => v.FieldValues
                    .Where(fv => fv.FieldDefinitionId == fieldDefinitionId)
                    .Select(fv => fv.ValueDate)
                    .FirstOrDefault());
        }

        private static IQueryable<DocumentVersion> ApplyEq(IQueryable<DocumentVersion> query, int defId, JsonElement filter)
        {
            if (!FilterJson.TryGetDateTimeValue(filter, out var value))
                return query;

            // Uzimamo samo datum (00:00:00)
            var dayStart = value.Date;
            var dayEnd = dayStart.AddDays(1);

            return query.Where(v => v.FieldValues.Any(fv =>
                fv.FieldDefinitionId == defId &&
                fv.ValueDate.HasValue &&
                fv.ValueDate.Value >= dayStart &&
                fv.ValueDate.Value < dayEnd));
        }

        private static IQueryable<DocumentVersion> ApplyCompare(
            IQueryable<DocumentVersion> query,
            int defId,
            JsonElement filter,
            string op)
        {
            if (!FilterJson.TryGetDateTimeValue(filter, out var value))
                return query;

            return op switch
            {
                ">" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDate > value)),
                ">=" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDate >= value)),
                "<" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDate < value)),
                "<=" => query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDate <= value)),
                _ => query
            };
        }

        private static IQueryable<DocumentVersion> ApplyBetween(IQueryable<DocumentVersion> query, int defId, JsonElement filter)
        {
            var from = FilterJson.GetDateTime(filter, "from");
            var to = FilterJson.GetDateTime(filter, "to");

            if (!from.HasValue && !to.HasValue)
                return query;

            if (from.HasValue)
                query = query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDate >= from.Value));

            if (to.HasValue)
                query = query.Where(v => v.FieldValues.Any(fv => fv.FieldDefinitionId == defId && fv.ValueDate <= to.Value));

            return query;
        }
    }
}
