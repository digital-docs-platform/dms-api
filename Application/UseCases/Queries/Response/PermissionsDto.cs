using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public class PermissionsDto
    {
        public string Code { get; set; } = null!;
        public Guid? DocumentTypeId { get; set; }
    }
}
