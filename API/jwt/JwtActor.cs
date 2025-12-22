using Application;

namespace API.jwt
{
    public class JwtActor : IApplicationActor
    {
        public int Id {  get; set; }

        public string Email { get; set; }

        public IReadOnlyCollection<string> ActorPermissions { get; set; }
    }
}
