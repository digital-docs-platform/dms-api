using Application.PermissionHandling;
using Application.Security.Cryptography;
using Application.Seeding;
using DataAccess;
using DocumentFormat.OpenXml.InkML;
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

            await SeedPermissionsAsync(ct);
            await SeedPermissionDependenciesAsync(ct);

            await SeedAdminAsync(ct);
            
        }

        public  async Task SeedPermissionsAsync(CancellationToken ct)
        {
            var codesFromCode = PermissionRegistry.GetAllCodes();

            var existing = await _db.Permissions
                .ToDictionaryAsync(p => p.Code, StringComparer.OrdinalIgnoreCase, ct);

            // 1) Upsert novih/postojećih
            foreach (var code in codesFromCode)
            {
                if (!existing.TryGetValue(code, out var perm))
                {
                    _db.Permissions.Add(new Permission
                    {
                        Code = code,
                        Scope = PermissionRegistry.GetScope(code),
                        Name = HumanizeCodeToName(code),
                        Description = null,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // 2) (Opcionalno) Deaktiviraj one koje više ne postoje u kodu
            var codeSet = codesFromCode.ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var perm in existing.Values)
            {
                if (!codeSet.Contains(perm.Code) && perm.IsActive)
                {
                    perm.IsActive = false;
                    perm.ModifiedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync(ct);
        }
        private async Task SeedPermissionDependenciesAsync(CancellationToken ct)
        {
            // code -> id
            var codeToId = await _db.Permissions
                .AsNoTracking()
                .ToDictionaryAsync(p => p.Code, p => p.Id, StringComparer.OrdinalIgnoreCase, ct);

            // existing pairs (composite key)
            var existing = await _db.PermissionDependencies
                .AsNoTracking()
                .Select(x => new { x.PermissionId, x.DependsOnPermissionId })
                .ToListAsync(ct);

            var existingSet = existing
                .Select(x => (x.PermissionId, x.DependsOnPermissionId))
                .ToHashSet();

            var toAdd = new List<PermissionDependency>();

            foreach (var (permCode, dependsOnCode) in PermissionRegistry.GetAllDependencies())
            {
                if (!codeToId.TryGetValue(permCode, out var permId))
                    throw new InvalidOperationException($"Permission '{permCode}' not found in DB. SeedPermissionsAsync must include it.");

                if (!codeToId.TryGetValue(dependsOnCode, out var depId))
                    throw new InvalidOperationException($"Dependency permission '{dependsOnCode}' not found in DB. SeedPermissionsAsync must include it.");

                if (permId == depId)
                    continue;

                if (existingSet.Contains((permId, depId)))
                    continue;

                toAdd.Add(new PermissionDependency
                {
                    PermissionId = permId,
                    DependsOnPermissionId = depId
                });
            }

            if (toAdd.Count == 0)
                return;

            _db.PermissionDependencies.AddRange(toAdd);
            await _db.SaveChangesAsync(ct);
        }



        public async Task SeedAdminAsync(CancellationToken ct)
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



        //Used to make Name for Permission out of PermissionCode(document.read) = Document Read
        private static string HumanizeCodeToName(string code)
        {
            // "documents.read" -> "Documents Read"
            // "document.versions.read" -> "Document Versions Read"
            var tokens = code
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => char.ToUpperInvariant(t[0]) + t[1..])
                .ToArray();

            return string.Join(' ', tokens);
        }
    }
}
