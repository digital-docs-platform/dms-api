using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Requests.User
{
    public class UpdateUserProfileRequest
    {
        public int UserId { get; set; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string? JobTitle { get; init; }
        public string? Department { get; init; }
    }
}
