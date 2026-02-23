using Application.Exceptions;
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
    public sealed class EFRemoveUserFromGroupCommand : EFUseCase, IRemoveUserFromGroupCommand
    {
        public string RequiredPermission => PermissionCodes.GroupRemoveUser;

        public PermissionScope Scope => PermissionScope.Group;

        public int Id => 22;

        public string Name => "Remove user from group";

        public string Description => "Remove user from user group";


        public EFRemoveUserFromGroupCommand(DatabaseContext context)
            : base(context)
        {
            
        }

        public async Task ExecuteAsync(RemoveUserFromGroupRequest request, CancellationToken ct)
        {
            if(!await _context.Groups.AnyAsync(g => g.Id == request.GroupId, ct))
            {
                throw new EntityNotFoundException("Group does not exist.");
            }

            if(!await _context.Users.AnyAsync(u => u.Id == request.UserId, ct))
            {
                throw new EntityNotFoundException("User does not exist.");
            }


            if (!await _context.UserGroups.AnyAsync(ug => ug.GroupId == request.GroupId && ug.UserId == request.UserId, ct))
            {
                throw new ConflictException("User is not in the group.");
            }

            var userGroup = await _context.UserGroups.FirstAsync(ug => ug.GroupId == request.GroupId && ug.UserId == request.UserId, ct);

            _context.UserGroups.Remove(userGroup);
            await _context.SaveChangesAsync(ct);
        }
    }
}
