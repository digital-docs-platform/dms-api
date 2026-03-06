using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public class GetDocumentAuditHistoryResponse : PagedResponse<GetDocumentAuditHistoryItem>
    {
        public List<GetDocumentAuditHistoryUserDto> Users { get; set; }
        public List<string> EventTypes { get; set; }
    }

    public class GetDocumentAuditHistoryItem
    {
        public Guid? PerformedById { get; set; }
        public string PerformedByFullName { get; set; } = string.Empty;
        public string PerformedByEmail { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;

        public string EntityType { get; set; } = string.Empty;
        public Guid? PerformedOnId { get; set; }
        public string? EntityName { get; set; }

        public string? Metadata { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetDocumentAuditHistoryUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
