using Application.UseCases.Commands.Requests.Common;
using Application.UseCases.Queries.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Requests.Group
{
    public class CreateGroupRequest
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PermissionRequest> Permissions { get; set; } = [];
    }
}
