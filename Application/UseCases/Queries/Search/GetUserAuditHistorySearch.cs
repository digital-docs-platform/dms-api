using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Search
{
    public class GetUserAuditHistorySearch
    {
        public List<AuditEventType>? EventTypes { get; set; }
        public Guid? PerformedById { get; set; }
        public string? PerformedByEmail { get; set; }
        public string? EntityType { get; set; }
        public Guid? PerformedOnId { get; set; }

        public int Days { get; set; } = 7;
        public SortDto Sort { get; set; } = new SortDto
        {
            Active = "CreatedAt",
            Direction = "desc"
        };
        public PaginationDto Pagination { get; set; } = new();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

    }

}
