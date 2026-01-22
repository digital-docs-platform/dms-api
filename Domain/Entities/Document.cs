using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Document : SoftDeletableEntity
    {
        public string Title { get; set; }
        public int DocumentTypeId { get; set; }
        public int CreatedBy { get; set; }
        public ICollection<DocumentVersion> DocumentVersions { get; set; } = new List<DocumentVersion>();

    }
}
