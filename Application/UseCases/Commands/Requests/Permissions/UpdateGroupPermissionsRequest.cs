using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Requests.Permissions
{
    public class UpdateGroupPermissionsRequest
    {
        public Guid GroupId { get; set; }
        public List<UpdateGroupPermissionsPermissionDto> Permissions { get; set; } = [];
    }

    public class UpdateGroupPermissionsPermissionDto
    {
        public int PermissionId { get; set; }
        public Guid? DocumentTypeId { get; set; }
    }
}
