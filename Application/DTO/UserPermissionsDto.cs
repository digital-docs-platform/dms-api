using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UserPermissionsDto
    {
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public int DocumentTypeId { get; set; } 
    }
}
