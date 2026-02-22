using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Requests.Common
{
    public class PermissionRequest
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public Guid? DocumentTypeId { get; set; }
    }
}
