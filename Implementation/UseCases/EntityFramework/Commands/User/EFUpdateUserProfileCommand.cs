using Application;
using Application.Exceptions;
using Application.Logging;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.User;
using DataAccess;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Commands.User
{
    public sealed class EFUpdateUserProfileCommand : EFUseCase, IUpdateUserProfileCommand
    {
        public string RequiredPermission => PermissionCodes.UsersUpdate;

        public PermissionScope Scope => PermissionScope.User;

        public int Id => 6;

        public string Name => "Update user info";

        public string Description => "Update user informations";

        public AuditLogEntry BuildAuditEntry(UpdateUserProfileRequest input, IApplicationActor actor)
        {
            return new AuditLogEntry
            {
                ActorEmail = actor.Email,
                ActorId = actor.Id,
                EntityId = input.UserId,
                EntityType = nameof(Domain.Entities.User),
                EntityName = $"{input.FirstName} {input.LastName}",
                EventType = AuditEventType.UserProfileUpdated,
                Metadata = new
                {
                    input.FirstName,
                    input.LastName,
                    input.Email,
                    input.JobTitle,
                    input.Department
                }
            };
        }
        public EFUpdateUserProfileCommand(DatabaseContext context)
            : base(context)
        {
            
        }

        public async Task ExecuteAsync(UpdateUserProfileRequest request, CancellationToken ct)
        {
            var user = await _context.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(ct);

            if (user == null)
                throw new EntityNotFoundException("User not found. . .");

            var email = request.Email.Trim();

            if (await _context.Users.AnyAsync(u => u.Id != request.UserId && u.Email == email, ct))
                throw new ConflictException("Email is already taken.");

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = email;
            user.JobTitle = string.IsNullOrWhiteSpace(request.JobTitle) ? null : request.JobTitle.Trim();
            user.Department = string.IsNullOrWhiteSpace(request.Department) ? null : request.Department.Trim();

            await _context.SaveChangesAsync();
        }

    }
}
