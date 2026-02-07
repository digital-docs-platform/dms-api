using Application;
using Application.PermissionHandling;

namespace Application.Jwt
{
    public class JwtActor : IApplicationActor
    {
        public Guid Id {  get; set; }

        public string Email { get; set; }

    }
}
