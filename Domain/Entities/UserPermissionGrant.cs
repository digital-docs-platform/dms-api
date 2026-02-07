using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserPermissionGrant : Entity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;

        public Guid? DocumentTypeId { get; set; }  // null => globalno
        public DocumentType? DocumentType { get; set; }

        public Guid? GrantedByUserId { get; set; }
        public User? GrantedByUser { get; set; }
    }
}
