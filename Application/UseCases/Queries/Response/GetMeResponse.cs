using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public sealed class GetMeResponse
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? JobTitle { get; set; }
        public string? Department { get; set; }

        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }

        public ICollection<GetMePermissionsResponse> Permissions { get; set; } = new List<GetMePermissionsResponse>();
        public GetMeUIResponse Ui { get; set; } = new();
    }

    public sealed class GetMePermissionsResponse
    {
        public string Code { get; set; } = null!;
        public Guid? DocumentTypeId { get; set; }
    }

    public sealed class GetMeUIResponse
    {
        public GetMeUICanResponse Can { get; set; } = new();
        public ICollection<GetMeLookupItemResponse> DocumentTypes { get; set; } = new List<GetMeLookupItemResponse>();
        public ICollection<GetMeLookupItemResponse> Users { get; set; } = new List<GetMeLookupItemResponse>();

        public ICollection<GetMeLookupItemResponse> Groups { get; set; } = new List<GetMeLookupItemResponse>();
    }

    public sealed class GetMeUICanResponse
    {
        public bool Users { get; set; }
        public bool Groups { get; set; }
        public bool DocumentTypes { get; set; }
    }

    // shared DTO za { id, name } (i DocumentTypes i Groups)
    public sealed class GetMeLookupItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
