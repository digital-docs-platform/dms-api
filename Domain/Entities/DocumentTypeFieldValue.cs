using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DocumentTypeFieldValue : Entity, IAuditable
    {
        public Guid VersionId { get; set; }
        public DocumentVersion Version { get; set; }

        public int FieldDefinitionId { get; set; }
        public DocumentTypeFieldDefinition FieldDefinition { get; set; }

        public string? ValueString { get; set; }
        public int? ValueInt { get; set; }
        public decimal? ValueDecimal { get; set; }
        public DateTime? ValueDate { get; set; }

        public int? ValueOptionId { get; set; }
        public DocumentTypeFieldOption? ValueOption { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
