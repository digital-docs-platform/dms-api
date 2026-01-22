using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PermissionHandling
{
    public sealed record UserPermissionsDto(
         string PermissionCode,
         int? DocumentTypeId
        );
}
