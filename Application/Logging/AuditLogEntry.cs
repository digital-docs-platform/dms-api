using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logging
{
    public class AuditLogEntry
    {
        public AuditEventType EventType { get; set; }
        public Guid? ActorId { get; set; }
        public string ActorEmail { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string? EntityName { get; set; }
        public object? Metadata { get; set; }   // serijalizuje se u JSON
        public string? IpAddress { get; set; }
    }
}
