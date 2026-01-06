using Application;

namespace API.Auth.jwt
{
    public class UnauthorizedActor : IApplicationActor
    {
        public int Id => 0;

        public string Email => "";

        public IReadOnlySet<string> ActorPermissions => new HashSet<string> {};
    }
}
