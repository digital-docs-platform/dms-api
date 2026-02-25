using Application;
using Application.Exceptions;
using Application.Logging;
using Application.PermissionHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Group;
using DataAccess;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Commands.Group
{
    public sealed class EFAddUserToGroupCommand : EFUseCase, IAddUserToGroupCommand
    {
        public string RequiredPermission => PermissionCodes.GroupAddUser;

        public PermissionScope Scope => PermissionScope.Group;

        public int Id => 21;

        public string Name => "Add user to group";

        public string Description => "Add user to group";
        public AuditLogEntry BuildAuditEntry(AddUserToGroupRequest input, IApplicationActor actor)
        {
            return new AuditLogEntry
            {
                ActorEmail = actor.Email,
                ActorId = actor.Id,
                EntityId = input.GroupId,
                EntityType = nameof(Domain.Entities.Group),
                EntityName = $"User added to Group with id: {input.GroupId}",
                EventType = AuditEventType.UserAddedToGroup,
                Metadata = new
                {
                    AddeduserId = input.UserId,
                    ToGroupId = input.GroupId
                }
            };
        }



        private readonly IApplicationActor _actor;

        public EFAddUserToGroupCommand(IApplicationActor actor, DatabaseContext context)
            : base(context)
        {
            _actor = actor;
        }
        public async Task ExecuteAsync(AddUserToGroupRequest request, CancellationToken ct)
        {
            if(!await _context.Groups.AnyAsync(g => g.Id == request.GroupId, ct))
                throw new EntityNotFoundException("Group not found.");

            if(!await _context.Users.AnyAsync(u => u.Id == request.UserId, ct))
                throw new EntityNotFoundException("User not found.");

            var alreadyAdded = await _context.UserGroups
                                             .AnyAsync(ug =>
                                                         ug.GroupId == request.GroupId && 
                                                         ug.UserId == request.UserId, ct);

            if (alreadyAdded)
                throw new EntityAlreadyExistsException($"User is already in this group.");

            var userGroup = new Domain.Entities.UserGroup
            {
                GroupId = request.GroupId,
                UserId = request.UserId,
                AddedAtUtc = DateTime.UtcNow,
                AddedByUserId = _actor.Id
            };

            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync(ct);


        }

    }
}
