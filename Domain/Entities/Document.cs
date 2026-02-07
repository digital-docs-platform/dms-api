using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Document : GuidEntity, ISoftDeletable, IAuditable
    {
        public string Title { get; set; }
        public Guid DocumentTypeId { get; set; }
        public Guid CreatedBy { get; set; }
        public User CreatedByUser { get; set; }
        public ICollection<DocumentVersion> DocumentVersions { get; set; } = new List<DocumentVersion>();
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
