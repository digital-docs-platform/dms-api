using Application.PermissionHandling.Resolver;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Search
{
    public class GetDocumentAuditHistorySearch : IHasDocumentId
    {
        public Guid DocumentId { get; set; }
        public Guid? PerformedById { get; set; }
        public List<AuditEventType>? EventTypes { get; set; }
        public SortDto Sort { get; set; } = new SortDto
        {
            Active = "CreatedAt",
            Direction = "desc"
        };
        public PaginationDto Pagination { get; set; } = new();

    }
}
