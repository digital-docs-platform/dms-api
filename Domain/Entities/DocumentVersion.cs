using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DocumentVersion : SoftDeletableEntity
    {
        public int DocumentId { get; set; }
        public int VersionNumber { get; set; }   // 1,2,3...
        public string? ChangeNote { get; set; }
        public bool IsCurrent { get; set; }
        public int CreatedBy { get; set; }

        // Fajl / sadržaj
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }

        // Gde je fajl smešten
        public string StorageKey { get; set; }   // npr "docs/2026/01/abc.pdf" ili S3 key


        public ICollection<DocumentTypeFieldValue> FieldValues { get; set; }
            = new List<DocumentTypeFieldValue>();
    }
}
