using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserGroup
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }

        public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid? AddedByUserId { get; set; }

    }
}
