using Application.PermissionHandling;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Implementation.PermissionHandling
{
    public class PermissionProvider : IPermissionProvider
    {
        private readonly DatabaseContext _db;

        public PermissionProvider(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<ICollection<UserPermissionsDto>> GetUserPermissionsAsync(Guid uid, CancellationToken ct = default)
        {
            var direct = await _db.UserPermissionGrants
                .AsNoTracking()
                .Where(x => x.UserId == uid)
                .Select(x => new UserPermissionsDto(x.Permission.Code, x.DocumentTypeId))
                .ToListAsync(ct);

            var fromGroups = await _db.GroupPermissions
                .AsNoTracking()
                .Where(gp => gp.Group.GroupUsers.Any(ug => ug.UserId == uid))
                .Select(gp => new UserPermissionsDto(gp.Permission.Code, gp.DocumentTypeId))
                .ToListAsync(ct);

            return direct
                .Concat(fromGroups)
                .Distinct() // radi jer je record (Code + DocumentTypeId)
                .ToList();
        }
    }
}
