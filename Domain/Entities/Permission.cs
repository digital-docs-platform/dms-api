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
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Module => Name.Split('.', 2)[0];
        ICollection<UserPermission> PermissionUsers { get; set; } = new List<UserPermission>();
        ICollection<GroupPermission> PermissionGroups { get; set; } = new List<GroupPermission>();
    }
}
