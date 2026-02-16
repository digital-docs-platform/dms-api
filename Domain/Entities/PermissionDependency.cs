using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PermissionDependency :  IAuditable
    {
        // Permission koja "ima" dependency (npr. document.versions.add)
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        // Permission od koje zavisi (npr. documents.read)
        public int DependsOnPermissionId { get; set; }
        public Permission DependsOnPermission { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
