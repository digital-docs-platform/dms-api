using Application.UseCases.Queries.Search;
using Domain.Entities;

namespace Implementation.Querying
{
    public static class AuditLogFilters
    {
        public static IQueryable<AuditLog> ApplyFilters(this IQueryable<AuditLog> query, AuditLogFiltersDto filters)
        {
            if (filters.ActorId.HasValue)
                query = query.Where(x => x.ActorId == filters.ActorId.Value);

            if (!string.IsNullOrEmpty(filters.PerformedByEmail))
                query = query.Where(x => x.ActorEmail == filters.PerformedByEmail);

            if (filters.EntityId.HasValue)
                query = query.Where(x => x.EntityId == filters.EntityId.Value);

            if (filters.EventTypes is { Count: > 0 })
                query = query.Where(x => filters.EventTypes.Contains(x.EventType));

            if (!string.IsNullOrEmpty(filters.EntityType))
                query = query.Where(x => x.EntityType == filters.EntityType);

            if (filters.DateFrom.HasValue || filters.DateTo.HasValue)
            {
                if (filters.DateFrom.HasValue)
                    query = query.Where(x => x.CreatedAt >= filters.DateFrom.Value);
                if (filters.DateTo.HasValue)
                    query = query.Where(x => x.CreatedAt <= filters.DateTo.Value);
            }
            else if (filters.Days.HasValue)
            {
                var from = DateTime.UtcNow.AddDays(-filters.Days.Value);
                query = query.Where(x => x.CreatedAt >= from);
            }

            return query;
        }

        public static IQueryable<AuditLog> ApplySort(this IQueryable<AuditLog> query, SortDto dto)
        {
            var desc = dto.Direction?.ToLower() == "desc";

            return dto.Active?.ToLower() switch
            {
                "performedbyemail" => desc ? query.OrderByDescending(x => x.ActorEmail) : query.OrderBy(x => x.ActorEmail),
                "eventtype"        => desc ? query.OrderByDescending(x => x.EventType)  : query.OrderBy(x => x.EventType),
                "entitytype"       => desc ? query.OrderByDescending(x => x.EntityType) : query.OrderBy(x => x.EntityType),
                _                  => desc ? query.OrderByDescending(x => x.CreatedAt)  : query.OrderBy(x => x.CreatedAt),
            };
        }

        public static IQueryable<AuditLog> ApplyPagination(this IQueryable<AuditLog> query, PaginationDto pagination)
        {
            return query
                .Skip(pagination.PageIndex * pagination.PageSize)
                .Take(pagination.PageSize);
        }
    }
}
