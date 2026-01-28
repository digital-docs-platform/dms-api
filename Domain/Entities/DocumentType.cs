using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DocumentType : Entity, ISoftDeletable, IAuditable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int AddedBy { get; set; }
        public ICollection<DocumentTypeFieldDefinition> FieldDefinitions { get; set; }
         = new List<DocumentTypeFieldDefinition>();
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
