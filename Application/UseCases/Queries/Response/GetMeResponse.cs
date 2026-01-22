using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public sealed class GetMeResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? JobTitle { get; set; }
        public string? Department {  get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
        public ICollection<GetMePermissionsResponse> Permissions { get; set; }
    }

    public sealed class GetMePermissionsResponse
    {
        public string Code { get; set; }
        public int? DocumentTypeId { get; set; }
    }
}
