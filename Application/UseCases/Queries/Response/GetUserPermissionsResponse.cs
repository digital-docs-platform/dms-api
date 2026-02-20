using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public  class GetUserPermissionsResponse
    {
        public List<PermissionsDto> Permissions { get; set; } = default!;
    }
}
