using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Group;
using DataAccess;
using DocumentFormat.OpenXml.Drawing;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Commands.Group
{
    public sealed class EFCreateGroupCommand : EFUseCase, ICreateGroupCommand
    {
        public string RequiredPermission => PermissionCodes.GroupWrite;

        public PermissionScope Scope => PermissionScope.Group;

        public int Id => 18;

        public string Name => "Create group";

        public string Description => "Create group with its own permissions that group users will inherit";



        public EFCreateGroupCommand(DatabaseContext context)
            : base(context)
        {
            
        }

        public async Task ExecuteAsync(CreateGroupRequest request, CancellationToken ct)
        {
            if (await _context.Groups.AnyAsync(g => g.Name == request.Name, ct))
                throw new EntityAlreadyExistsException($"Group '{request.Name}' already exists.");

            var permissionIds = request.Permissions
                .Select(p => p.Id)
                .Distinct()
                .ToList();

            if (permissionIds.Count != request.Permissions.Count)
                throw new ConflictException("Duplicate permissions are not allowed.");

            var permissions = await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync(ct);

            if (permissions.Count != permissionIds.Count)
                throw new ConflictException("One or more permissions do not exist.");

            var dependencies = await _context.PermissionDependencies
                                            .Where(d => permissionIds.Contains(d.PermissionId))
                                            .ToListAsync(ct);

            var missingDependencies = new List<string>();

            foreach (var dependency in dependencies)
            {
                if (!permissionIds.Contains(dependency.DependsOnPermissionId))
                {
                    var missingPermission = await _context.Permissions
                        .Where(p => p.Id == dependency.DependsOnPermissionId)
                        .Select(p => p.Code)
                        .FirstAsync(ct);

                    missingDependencies.Add(missingPermission);
                }
            }

            if (missingDependencies.Any())
            {
                throw new ConflictException(
                    $"Missing required permissions: {string.Join(", ", missingDependencies)}"
                );
            }

            var group = new Domain.Entities.Group
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                GroupPermissions = request.Permissions.Select(p => new GroupPermission
                {
                    PermissionId = p.Id,
                    DocumentTypeId = p.DocumentTypeId,
                }).ToList(),
                IsActive = true
            };

            await _context.Groups.AddAsync(group, ct);
            await _context.SaveChangesAsync(ct);





        }
    }
}
