using Application;

namespace API.jwt
{
    public class UnauthorizedActor : IApplicationActor
    {
        public int Id => 0;

        public string Email => "";

        public IReadOnlyCollection<string> ActorPermissions => new List<string> {};
    }
}
