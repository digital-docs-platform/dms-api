using Application;
using Application.PermissionHandling;

namespace Application.Jwt
{
    public class UnauthorizedActor : IApplicationActor
    {
        public int Id => 0;

        public string Email => "";

        //public IReadOnlySet<UserPermissionsDto> ActorPermissions => new HashSet<UserPermissionsDto> {};
    }
}
