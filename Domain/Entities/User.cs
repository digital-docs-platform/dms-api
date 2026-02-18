using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : GuidEntity, IActivatable, ISoftDeletable, IAuditable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }    
        public string PasswordHash { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public bool IsLocked { get; set; } = false;
        public DateTime? LastLoginUtc {  get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        //public ICollection<UserPermissionGrant> UserPermissions { get; set; } = new List<UserPermissionGrant>();
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<GroupPermission> GroupPermissionsAddedByMe { get; set; } = new List<GroupPermission>();


    }
}
