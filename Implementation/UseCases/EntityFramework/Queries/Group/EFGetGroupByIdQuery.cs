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

namespace Implementation.UseCases.EntityFramework.Queries.Group
{
    public sealed class EFGetGroupByIdQuery : EFUseCase, IGetGroupByIdQuery
    {
        public string RequiredPermission => PermissionCodes.GroupsRead;

        public PermissionScope Scope => PermissionScope.Group;

        public int Id => 20;

        public string Name => "Get Group by id";

        public string Description => "Get group information including group users";

        public EFGetGroupByIdQuery(DatabaseContext context)
            : base(context)
        {
            
        }

        public async Task<GetGroupByIdResponse> ExecuteAsync(IdSearch<Guid> search, CancellationToken ct)
        {
            if (await _context.Groups.Include(g => g.GroupUsers)
                                     .ThenInclude(gu => gu.User)
                                     .FirstOrDefaultAsync(g => g.Id == search.Id, ct) is Domain.Entities.Group group)
            {
                return new GetGroupByIdResponse
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Users = group.GroupUsers.Select(u => new GroupUserDto
                    {
                        Id = u.User.Id,
                        FirstName = u.User.FirstName,
                        LastName = u.User.LastName,
                        Email = u.User.Email,
                        JobTitle = u.User.JobTitle,
                        Department = u.User.Department
                    }).ToList()
                };
            }
            else
            {
                throw new EntityNotFoundException($"Group not found");
            }




        }
    }
}
