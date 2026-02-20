using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Requests.Permissions
{
    public  class UpdateUserPermissionsRequest
    {
        public Guid UserId { get; set; }
        public List<UpdateUserPermissionsPermissionDto> Permissions { get; set; } = [];
    }

    public class UpdateUserPermissionsPermissionDto
    {
        public int PermissionId { get; set; }
        public Guid? DocumentTypeId { get; set; }
    }
}
