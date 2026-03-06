using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Domain.Enums;
using Implementation.Querying;
using Microsoft.EntityFrameworkCore;

namespace Implementation.UseCases.EntityFramework.Queries.Audit
{
    public sealed class EFGetDocumentAuditHistoryQuery : EFUseCase, IGetDocumentAuditHistoryQuery
    {
        public string RequiredPermission => PermissionCodes.DocumentsAuditRead;

        public PermissionScope Scope => PermissionScope.Document;

        public int Id => 30;

        public string Name => "Get document audit";

        public string Description => "Get document audit history";

        public EFGetDocumentAuditHistoryQuery(DatabaseContext context)
            : base(context)
        {
        }

        public async Task<GetDocumentAuditHistoryResponse> ExecuteAsync(GetDocumentAuditHistorySearch search, CancellationToken ct)
        {
            if (!await _context.Documents.AnyAsync(d => d.Id == search.DocumentId, ct))
                throw new EntityNotFoundException("Document not found.");

            var baseQuery = _context.AuditLogs
                .Where(al => al.EntityId == search.DocumentId && al.EntityType == "Document")
                .AsQueryable();

            var actorIds = await baseQuery
                .Where(x => x.ActorId.HasValue)
                .Select(x => x.ActorId!.Value)
                .Distinct()
                .ToListAsync(ct);

            var users = await _context.Users
                .Where(u => actorIds.Contains(u.Id))
                .Select(u => new GetDocumentAuditHistoryUserDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName
                })
                .ToListAsync(ct);

            var eventTypes = await baseQuery
                .Select(x => x.EventType.ToString())
                .Distinct()
                .ToListAsync(ct);

            var query = baseQuery.ApplyFilters(new AuditLogFiltersDto
            {
                ActorId = search.PerformedById,
                EventTypes = search.EventTypes
            });

            query = query.ApplySort(search.Sort);

            var totalCount = await query.CountAsync(ct);

            var pageIndex = search.Pagination?.PageIndex ?? 0;
            var pageSize = search.Pagination?.PageSize ?? 10;

            query = query.ApplyPagination(new PaginationDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            var items = await query
                .Select(x => new GetDocumentAuditHistoryItem
                {
                    EventType = x.EventType.ToString(),
                    PerformedById = x.ActorId,
                    PerformedByEmail = x.ActorEmail,
                    EntityType = x.EntityType,
                    PerformedOnId = x.EntityId,
                    EntityName = x.EntityName,
                    Metadata = x.Metadata,
                    IpAddress = x.IpAddress,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(ct);

            foreach (var item in items)
            {
                item.PerformedByFullName = users
                    .FirstOrDefault(u => u.Id == item.PerformedById)?.FullName ?? string.Empty;
            }

            return new GetDocumentAuditHistoryResponse
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Users = users,
                EventTypes = eventTypes
            };
        }
    }
}
