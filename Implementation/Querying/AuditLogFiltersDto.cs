using Domain.Enums;

namespace Implementation.Querying
{
    public class AuditLogFiltersDto
    {
        public Guid? ActorId { get; set; }
        public string? PerformedByEmail { get; set; }
        public Guid? EntityId { get; set; }
        public List<AuditEventType>? EventTypes { get; set; }
        public string? EntityType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? Days { get; set; }
    }
}
