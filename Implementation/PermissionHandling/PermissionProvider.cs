using Application.PermissionHandling;
using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.PermissionHandling
{
    public class PermissionProvider : IPermissionProvider
    {
        private readonly DatabaseContext _db;
        public PermissionProvider(DatabaseContext db) 
        {
            _db = db;
        }
        public async Task<ICollection<UserPermissionsDto>> GetUserPermissionsAsync(int uid, CancellationToken ct = default)
        {
            var list = await _db.UserPermissionGrants
                 .Where(x => x.UserId == uid)
                 .Select(x => new UserPermissionsDto
                 (
                     x.Permission.Code,
                     x.DocumentTypeId
                 ))
                 .ToListAsync(ct);

            return list;
        }
    }
}
