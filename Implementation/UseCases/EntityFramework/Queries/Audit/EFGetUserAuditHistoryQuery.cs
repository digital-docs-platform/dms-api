using Application.PermissionHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using DocumentFormat.OpenXml.Features;
using Domain.Enums;
using Implementation.Querying.UserAuditHistoryQuerying;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.Audit
{
    public sealed class EFGetUserAuditHistoryQuery : EFUseCase, IGetUserAuditHistoryQuery
    {
        public string RequiredPermission => PermissionCodes.SystemAdmin;

        public PermissionScope Scope => PermissionScope.System;

        public int Id => 26;

        public string Name => "Get user audit";

        public string Description => "Returns all info about event history";
        public EFGetUserAuditHistoryQuery(DatabaseContext context)
            : base(context)
        {
            
        }

        public async Task<GetUserAuditHistoryResponse> ExecuteAsync(GetUserAuditHistorySearch search, CancellationToken ct)
        {

            var pageIndex = search.Pagination?.PageIndex ?? 0;
            var pageSize = search.Pagination?.PageSize ?? 10;

            if (pageIndex < 0) pageIndex = 0;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.AuditLogs.AsQueryable();

            query = query.ApplyFilters(search);
            query = query.ApplySort(search.Sort);

            var totalCount = await query.CountAsync(ct);

            query = query.ApplyPagination(new PaginationDto
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
            });



            var users = await _context.Users.Select(u => new GetUserAuditHistoryUserDto
            {
                FullName = u.FirstName + " " + u.LastName,
                Id = u.Id
            }).ToListAsync(ct);



            var entityTypes = await _context.AuditLogs.Select(al => al.EntityType).ToListAsync(ct);
            
            var distinctEntityTypes = entityTypes.Distinct().ToList();

            var eventTypes = Enum.GetNames<AuditEventType>().ToList();


            var items = await query
                 .Select(x => new UserAuditHistoryItem
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


            return new GetUserAuditHistoryResponse
            {
                Users = users,
                EntityTypes = distinctEntityTypes,
                EventTypes = eventTypes,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
