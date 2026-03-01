using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public class GetUserAuditHistoryResponse  : PagedResponse<UserAuditHistoryItem>
    {
        public List<GetUserAuditHistoryUserDto> Users {  get; set; }
        public List<string> EntityTypes { get; set; }
        public List<string> EventTypes {  get; set; }

    }

    public class UserAuditHistoryItem
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

    public class GetUserAuditHistoryUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;

    }
}
