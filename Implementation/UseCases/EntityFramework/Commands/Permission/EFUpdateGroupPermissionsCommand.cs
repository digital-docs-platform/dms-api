using Application;
using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Permissions;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Implementation.PermissionHandling;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Commands.Permission
{
    public sealed class EFUpdateGroupPermissionsCommand : EFUseCase, IUpdateGroupPermissionsCommand
    {
        public string RequiredPermission => PermissionCodes.SystemAdmin;

        public PermissionScope Scope => PermissionScope.System;

        public int Id => 24;

        public string Name => "Update group permissions";

        public string Description => "Update group permissions (snapshot)";

        private readonly IPermissionSnapshotService _permissionSnapshotService;
        private readonly IApplicationActor _actor;
        public EFUpdateGroupPermissionsCommand(
                                                IPermissionSnapshotService permissionSnapshotService,
                                                IApplicationActor actor,
                                                DatabaseContext context)
            : base(context)
        {
            _permissionSnapshotService = permissionSnapshotService;
            _actor = actor;
        }


        public async Task ExecuteAsync(UpdateGroupPermissionsRequest request, CancellationToken ct)
        {
         
            var groupExists = await _context.Groups.AnyAsync(u => u.Id == request.GroupId, ct);
            if (!groupExists)
                throw new EntityNotFoundException("Group not found.");

            
            var currentGrants = await _context.GroupPermissions
                .Where(x => x.GroupId == request.GroupId)
                .ToListAsync(ct);

            // Validate requested permissions exist
            var allPermissionIds = (await _context.Permissions.Select(p => p.Id).ToListAsync(ct)).ToHashSet();
            foreach (var p in request.Permissions)
            {
                if (!allPermissionIds.Contains(p.PermissionId))
                    throw new EntityNotFoundException("Permission does not exist.");
            }

            
            var requestedKeys = request.Permissions
                .Select(p => new PermissionKey
                {
                    PermissionId = p.PermissionId,
                    DocumentTypeId = p.DocumentTypeId
                })
                .ToList();

            var requestedPermissionIds = requestedKeys
                .Select(k => k.PermissionId)
                .ToHashSet();

            var currentPermissionIds = currentGrants
                .Select(g => g.PermissionId)
                .ToHashSet();

            // Effective permissions are those already granted plus those being requested
            var effectivePermissionIds = requestedPermissionIds.Union(currentPermissionIds).ToHashSet();

            // Load dependencies for requested permissions
            var dependencies = await _context.PermissionDependencies
                                            .Where(d => requestedPermissionIds.Contains(d.PermissionId))
                                            .ToListAsync(ct);

            var missingDependencies = new List<string>();

            foreach (var dependency in dependencies)
            {
                // If the dependency is not present in the effective set, it's missing
                if (!effectivePermissionIds.Contains(dependency.DependsOnPermissionId))
                {
                    var missingPermissionCode = await _context.Permissions
                        .Where(p => p.Id == dependency.DependsOnPermissionId)
                        .Select(p => p.Code)
                        .FirstOrDefaultAsync(ct);

                   
                    missingDependencies.Add(missingPermissionCode ?? dependency.DependsOnPermissionId.ToString());
                }
            }

            if (missingDependencies.Any())
            {
                throw new ConflictException(
                    $"Missing required permissions: {string.Join(", ", missingDependencies)}"
                );
            }


            var (toRemove, toAdd) = _permissionSnapshotService.Reconcile(
                currentGrants,
                requestedKeys,
                g => g.PermissionId,
                g => g.DocumentTypeId);

            _context.GroupPermissions.RemoveRange(toRemove);

            foreach (var key in toAdd)
            {
                _context.GroupPermissions.Add(new GroupPermission
                {
                    PermissionId = key.PermissionId,
                    DocumentTypeId = key.DocumentTypeId,
                    AddedByUserId = _actor.Id,
                    AddedAtUtc = DateTime.UtcNow,
                    GroupId = request.GroupId
                });
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
