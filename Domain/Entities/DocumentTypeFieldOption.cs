using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DocumentTypeFieldOption : Entity, ISoftDeletable, IAuditable
    {
        public int FieldDefinitionId { get; set; }
        public DocumentTypeFieldDefinition FieldDefinition { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string Label { get; set; } = default!;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
