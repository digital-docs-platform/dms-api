using Domain.Entities.BaseEntities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DocumentTypeFieldDefinition : SoftDeletableEntity
    {
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }

        public FieldDataType DataType { get; set; }

        public bool IsRequired { get; set; }
        public int SortOrder { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsSortable { get; set; }

    }
}
