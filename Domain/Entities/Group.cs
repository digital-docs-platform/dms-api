using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Group : GuidEntity, ISoftDeletable, IAuditable, IActivatable
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }


        public ICollection<UserGroup> GroupUsers { get; set; } = new List<UserGroup>();
        public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
    }
}
