using Domain.Entities.BaseEntities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuditLog : GuidEntity
    {
        public AuditEventType EventType { get; set; }

        // Who did the action
        public Guid? ActorId { get; set; }
        public string ActorEmail { get; set; } = string.Empty;

        // On what entity
        public string EntityType { get; set; } = string.Empty;  // "Document", "User", "Group"
        public Guid? EntityId { get; set; }
        public string? EntityName { get; set; }  // denorm naziv (title dokumenta, email usera...)

        public string? Metadata { get; set; }    // JSON string
        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
