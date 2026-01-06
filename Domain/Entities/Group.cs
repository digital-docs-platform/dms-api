using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Group : SoftDeletableActivatableEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        //ICollection<UserGroup> GroupUsers { get; set; } = new List<UserGroup>();
        //ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
    }
}
