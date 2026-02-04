using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying
{
    public sealed class FieldQueryDispatcher : IFieldQueryDispatcher
    {
        private readonly Dictionary<FieldDataType, IFieldQueryStrategy> _strategies;

        public FieldQueryDispatcher(IEnumerable<IFieldQueryStrategy> strategies)
        {
            // Ako se desi duplikat tipova, ovo će puknuti odmah pri startu (što je dobro)
            _strategies = strategies.ToDictionary(s => s.DataType);
        }

        public IQueryable<DocumentVersion> ApplyFilter(
            IQueryable<DocumentVersion> query,
            DocumentTypeFieldDefinition definition,
            JsonElement filter)
        {
            if(!definition.IsSearchable)
                return query;

            // Safety: ako nema strategije za taj tip, ignoriši filter
            return _strategies.TryGetValue(definition.DataType, out var strategy)
                ? strategy.ApplyFilter(query, definition.Id, filter)
                : query;
        }

        public IQueryable<DocumentVersion> ApplySort(
            IQueryable<DocumentVersion> query,
            DocumentTypeFieldDefinition definition,
            bool desc)
        {
            if (!definition.IsSortable) return query;

            if (!_strategies.TryGetValue(definition.DataType, out var strategy))
                return query;

            if (!strategy.CanSort)
                return query;

            return strategy.ApplySort(query, definition.Id, desc);
        }
    }
}
