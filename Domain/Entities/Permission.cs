using Domain.Entities.BaseEntities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Permission : Entity, IAuditable, IActivatable
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Module => Code.Split('.', 2)[0];
        public PermissionScope Scope {  get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserPermissionGrant> PermissionUsers { get; set; } = new List<UserPermissionGrant>();
        public ICollection<GroupPermission> PermissionGroups { get; set; } = new List<GroupPermission>();



        // Ova permission "zahteva" druge
        public ICollection<PermissionDependency> Dependencies { get; set; } = new List<PermissionDependency>();

        // Druge permisije koje "zavise" od ove (inverse)
        public ICollection<PermissionDependency> DependedOnBy { get; set; } = new List<PermissionDependency>();
    }
}
