using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : SoftDeletableActivatableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }    
        public string PasswordHash { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public bool IsLocked { get; set; } = false;
        public DateTime? LastLoginUtc {  get; set; }

        //public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        //public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
       

    }
}
