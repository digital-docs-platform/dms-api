using Domain.Entities.BaseEntities;
using Domain.Enums;

namespace Domain.Entities
{
    public class DocumentVersionFile : GuidEntity, IAuditable
    {
        public Guid DocumentVersionId { get; set; }
        public DocumentVersion Version { get; set; } = default!;

        public string Name { get; set; } = default!;
        public FileExtension Extension { get; set; }
        public string ContentType { get; set; } = default!;
        public long SizeInBytes { get; set; }
        public string StorageKey { get; set; } = default!;
        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
