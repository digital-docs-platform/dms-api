using Application;
using Application.Exceptions;
using Application.Logging;
using Application.PermissionHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Permissions;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Implementation.PermissionHandling;
using Microsoft.EntityFrameworkCore;

namespace Implementation.UseCases.EntityFramework.Commands.Permission
{
    public sealed class EFUpdateUserPermissionsCommand : EFUseCase, IUpdateUserPermissionsCommand
    {
        public string RequiredPermission => PermissionCodes.SystemAdmin;
        public PermissionScope Scope => PermissionScope.System;
        public int Id => 13;
        public string Name => "Update User permissions";
        public string Description => "Replace user permission grants (snapshot)";

        private readonly IApplicationActor _actor;
        private readonly IPermissionSnapshotService _permissionSnapshotService;

        public EFUpdateUserPermissionsCommand(
                                            IApplicationActor actor, 
                                            IPermissionSnapshotService permissionSnapshotService,
                                            DatabaseContext context)
            : base(context)
        {
            _actor = actor;
            _permissionSnapshotService = permissionSnapshotService;
        }

        public async Task ExecuteAsync(UpdateUserPermissionsRequest request, CancellationToken ct)
        {

            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, ct);
            if (!userExists)
                throw new EntityNotFoundException("User not found.");

            var currentGrants = await _context.UserPermissionGrants
                .Where(x => x.UserId == request.UserId)
                .ToListAsync(ct);

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

            
            var (toRemove, toAddKeys) = _permissionSnapshotService.Reconcile(
                currentGrants,
                requestedKeys,
                g => g.PermissionId,
                g => g.DocumentTypeId);

            _context.UserPermissionGrants.RemoveRange(toRemove);

            foreach (var key in toAddKeys)
            {
                _context.UserPermissionGrants.Add(new UserPermissionGrant
                {
                    UserId = request.UserId,
                    PermissionId = key.PermissionId,
                    DocumentTypeId = key.DocumentTypeId,
                    GrantedByUserId = _actor.Id
                });
            }

            await _context.SaveChangesAsync(ct);
        }

        public AuditLogEntry BuildAuditEntry(UpdateUserPermissionsRequest input, IApplicationActor actor)
        {
           return new AuditLogEntry
            {
                ActorEmail = actor.Email,
                ActorId = actor.Id,
                EntityId = input.UserId,
                EntityType = nameof(Domain.Entities.User),
                EntityName = $"User permissions updated for user with id: {input.UserId}",
                EventType = AuditEventType.UserPermissionsUpdated,
                Metadata = new
                {
                    input.UserId,
                    UpdatedPermissions = input.Permissions.Select(p => new
                    {
                        p.PermissionId,
                        p.DocumentTypeId
                    })
                }
            };
        }
    }
}
