using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GroupPermission
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;

        public Guid? AddedByUserId { get; set; }
        public User AddedByUser { get; set; } = null!;
    }
}
