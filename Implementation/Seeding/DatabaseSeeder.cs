using Application.PermissionHandling;
using Application.Security.Cryptography;
using Application.Seeding;
using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Seeding
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly DatabaseContext _db;
        private readonly IPasswordHasher _hasher;

        public DatabaseSeeder(DatabaseContext db, IPasswordHasher hasher)
        {
            _db = db;
            _hasher = hasher;
        }
        public async Task SeedAsync(CancellationToken ct)
        {
            const string email = "admin@admin.com";
            const string password = "@dmin123";

            // admin user exists
            var admin = await _db.Users.SingleOrDefaultAsync(x => x.Email == email, ct);
            if (admin is null)
            {
                admin = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "Adminovich",
                    Email = email,
                    PasswordHash = _hasher.Hash(password),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(admin);
                await _db.SaveChangesAsync(ct);
            }

            //Ensure system.admin permission exists in DB (seeded via HasData)
            var adminPermCode = PermissionCodes.SystemAdmin;
            var adminPermId = await _db.Permissions
                .AsNoTracking()
                .Where(p => p.Code == adminPermCode)
                .Select(p => (int?)p.Id)
                .SingleOrDefaultAsync(ct);

            if (adminPermId is null)
                throw new InvalidOperationException($"Permission '{adminPermCode}' not found. Seed permissions first.");

            // Ensure grant exists
            var alreadyGranted = await _db.UserPermissionGrants
                .AsNoTracking()
                .AnyAsync(g => g.UserId == admin.Id && g.PermissionId == adminPermId.Value, ct);

            if (alreadyGranted)
                return;

            _db.UserPermissionGrants.Add(new UserPermissionGrant
            {
                UserId = admin.Id,
                PermissionId = adminPermId.Value,
                DocumentTypeId = null,
                GrantedByUserId = admin.Id
            });

            await _db.SaveChangesAsync(ct);
        }
    }
}
