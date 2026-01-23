using Application;
using Application.Exceptions;
using Application.Jwt;
using Application.PermissionHandling;

namespace Implementation.PermissionHandling
{
    public class PermissionHandler : IPermissionHandler
    {
        private readonly IApplicationActor _actor;
        private readonly IPermissionProvider _permissionProvider;

        public PermissionHandler(IApplicationActor actor, IPermissionProvider permissionProvider)
        {
            _actor = actor;
            _permissionProvider = permissionProvider;
        }

        public async Task EnsureAsync(string permissionCode, int? documentTypeId, CancellationToken ct = default)
        {
            if (_actor is UnauthorizedActor || _actor.Id <= 0)
                throw new UnauthorizedException("User is not authenticated.");

            var perms = await _permissionProvider.GetUserPermissionsAsync(_actor.Id, ct);

            if(perms.Any(p => p.PermissionCode == PermissionCodes.SystemAdmin)) 
                return;

            bool allowed =
                perms.Any(p => p.PermissionCode == permissionCode
                               && (p.DocumentTypeId == documentTypeId || p.DocumentTypeId is null));

            if (!allowed)
                throw new UnauthorizedException("Access denied.");
        }
    }
}
