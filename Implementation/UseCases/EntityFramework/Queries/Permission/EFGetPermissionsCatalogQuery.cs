using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.Permission
{
    public sealed class EFGetPermissionsCatalogQuery : EFUseCase, IGetPermissionsCatalogQuery
    {
        public string RequiredPermission => PermissionCodes.SystemAdmin;

        public PermissionScope Scope => PermissionScope.System;

        public int Id => 12;

        public string Name => "Get permissions catalog";

        public string Description => "Get all permissions";

        private readonly IPermissionProvider _permissionProvider;

        public EFGetPermissionsCatalogQuery(IPermissionProvider permissionProvider, DatabaseContext context)
            : base(context)
        {
            _permissionProvider = permissionProvider;
        }

        public async Task<GetPermissionCatalogResponse> ExecuteAsync(IdSearch search, CancellationToken ct)
        {

            if (!await _context.Users.AnyAsync(u => u.Id == search.Id))
                throw new EntityNotFoundException("User not found...");




            // 1) sve permisije
            var permissions = await _context.Permissions
                .AsNoTracking()
                .Where(p => p.IsActive && p.Scope != PermissionScope.System)
                .Select(p => new
                {
                    p.Id,
                    p.Code,
                    p.Name,
                    p.Description,
                    p.Scope
                })
                .ToListAsync(ct);

            // 2) dependencies: (permissionCode -> list dependsOnCodes)
            var deps = await _context.PermissionDependencies
                .AsNoTracking()
                .Select(d => new { d.PermissionId, d.DependsOnPermissionId })
                .ToListAsync(ct);

            // map id -> code (za dependency translate)
            var idToCode = permissions.ToDictionary(p => p.Id, p => p.Code);

            var dependsOnByCode = deps
                .Where(x => idToCode.ContainsKey(x.PermissionId) && idToCode.ContainsKey(x.DependsOnPermissionId))
                .GroupBy(x => idToCode[x.PermissionId])
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => idToCode[x.DependsOnPermissionId]).Distinct().ToList(),
                    StringComparer.OrdinalIgnoreCase
                );

            // 3) user grants (merge user+group radiš server-side kasnije; za sada user grants)
            var userPerms = (await _permissionProvider.GetUserPermissionsAsync(search.Id, ct))
                            .Select(p => new UserGrantedPermissionDto 
                            { Code = p.PermissionCode, DocumentTypeId = p.DocumentTypeId })
                            .ToList();

            // 4) složi scope grupisano
            var scopeGroups = permissions
                .GroupBy(p => p.Scope)
                .OrderBy(g => g.Key) // po enum vrednosti, ili custom order
                .Select(g => new PermissionScopeGroupDto
                {
                    Scope = g.Key,
                    Permissions = g
                        .OrderBy(x => x.Name)
                        .Select(x => new PermissionItemDto
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            Description = x.Description,
                            DependsOn = dependsOnByCode.TryGetValue(x.Code, out var list) ? list : new List<string>()
                        })
                        .ToList()
                })
                .ToList();

            var documentTypes = await _context.DocumentTypes.Select(dt => new DocumentTypesDto
            {
                Id = dt.Id,
                Name = dt.Name
            }).ToListAsync(ct);

            return new GetPermissionCatalogResponse
            {
                Scopes = scopeGroups,
                UserPermissions = userPerms,
                DocumentTypes = documentTypes
            };
        }
    }
}
