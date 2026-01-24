using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public sealed class GetUserByIdResponse
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? JobTitle { get; set; }
        public string? Department { get; set; }

        public string Email { get; set; } = null!;
    }
}
