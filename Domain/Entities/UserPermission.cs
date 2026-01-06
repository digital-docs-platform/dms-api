using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserPermission
    {
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;
        public int? AddedByUserId { get; set; }
    }
}
