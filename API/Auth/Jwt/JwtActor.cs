using Application;

namespace API.Auth.jwt
{
    public class JwtActor : IApplicationActor
    {
        public int Id {  get; set; }

        public string Email { get; set; }

        public IReadOnlySet<string> ActorPermissions { get; set; }
    }
}
