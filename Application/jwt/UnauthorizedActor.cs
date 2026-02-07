using Application;
using Application.PermissionHandling;

namespace Application.Jwt
{
    public class UnauthorizedActor : IApplicationActor
    {
        public Guid Id => default;

        public string Email => "";

        //public IReadOnlySet<UserPermissionsDto> ActorPermissions => new HashSet<UserPermissionsDto> {};
    }
}
