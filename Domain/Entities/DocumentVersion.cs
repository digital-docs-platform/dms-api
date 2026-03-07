using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DocumentVersion : GuidEntity, ISoftDeletable, IAuditable
    {
        public Guid DocumentId { get; set; }
        public Document DocumentInstance { get; set; } = default!;
        public int VersionNumber { get; set; }   // 1,2,3...
        public string? ChangeNote { get; set; }
        public bool IsCurrent { get; set; }
        public Guid CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public ICollection<DocumentVersionFile> Files { get; set; } = new List<DocumentVersionFile>();
        public ICollection<DocumentTypeFieldValue> FieldValues { get; set; }
            = new List<DocumentTypeFieldValue>();
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
