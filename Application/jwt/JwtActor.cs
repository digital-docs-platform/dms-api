using Application;
using Application.PermissionHandling;

namespace Application.Jwt
{
    public class JwtActor : IApplicationActor
    {
        public int Id {  get; set; }

        public string Email { get; set; }

    }
}
