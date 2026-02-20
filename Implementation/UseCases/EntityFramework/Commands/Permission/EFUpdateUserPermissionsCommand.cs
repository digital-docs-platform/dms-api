using Application;
using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Permissions;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
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

        public EFUpdateUserPermissionsCommand(IApplicationActor actor, DatabaseContext context)
            : base(context)
        {
            _actor = actor;
        }

        public async Task ExecuteAsync(UpdateUserPermissionsRequest request, CancellationToken ct)
        {
           
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, ct);
            if (!userExists)
                throw new EntityNotFoundException("User not found.");

           
            var currentGrants = await _context.UserPermissionGrants
                .Where(x => x.UserId == request.UserId)
                .ToListAsync(ct);

            
            var allPermissionIds = await _context.Permissions
                .Select(p => p.Id)
                .ToListAsync(ct);

            foreach (var p in request.Permissions)
            {
                if (!allPermissionIds.Contains(p.PermissionId))
                    throw new EntityNotFoundException("Permission does not exist.");
            }

            // 4) remove grants that are NOT in request (by PermissionId + DocumentTypeId)
            var toRemove = currentGrants
                .Where(g => !request.Permissions.Any(p =>
                    p.PermissionId == g.PermissionId &&
                    p.DocumentTypeId == g.DocumentTypeId))
                .ToList();

            _context.UserPermissionGrants.RemoveRange(toRemove);

            // 5) add grants that are in request but NOT in db
            foreach (var p in request.Permissions)
            {
                var alreadyExists = currentGrants.Any(g =>
                    g.PermissionId == p.PermissionId &&
                    g.DocumentTypeId == p.DocumentTypeId);

                if (alreadyExists)
                    continue;

                _context.UserPermissionGrants.Add(new UserPermissionGrant
                {
                    UserId = request.UserId,
                    PermissionId = p.PermissionId,
                    DocumentTypeId = p.DocumentTypeId,
                    GrantedByUserId = _actor.Id
                });
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
