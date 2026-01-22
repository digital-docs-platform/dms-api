using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Permission : AuditableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Module => Code.Split('.', 2)[0];
        ICollection<UserPermissionGrant> PermissionUsers { get; set; } = new List<UserPermissionGrant>();
        ICollection<GroupPermission> PermissionGroups { get; set; } = new List<GroupPermission>();
    }
}
