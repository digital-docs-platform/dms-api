using Application;
using Application.Exceptions;
using Application.Jwt;
using Application.PermissionHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.User
{
    public class EFGetMeQuery : EFUseCase, IGetMeQuery
    {
        public int Id => 2;

        public string Name => "User Informations";

        public string Description => "Get user informations";

        private readonly IApplicationActor _actor;
        private readonly IPermissionProvider _permissionProvider;
        public EFGetMeQuery(IApplicationActor actor, DatabaseContext context, IPermissionProvider permissionProvider)
            : base(context)
        {
            _actor = actor;
            _permissionProvider = permissionProvider;
        }

        public async Task<GetMeResponse> ExecuteAsync(EmptySearch search, CancellationToken ct)
        {

            if (_actor is UnauthorizedActor || _actor.Id <= 0)
                throw new UnauthenticatedException("User is not authenticated.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _actor.Id, ct);
            if (user is null)
                throw new EntityNotFoundException("User not found."); 

            var perms = await _permissionProvider.GetUserPermissionsAsync(user.Id, ct);

            var response = new GetMeResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Department = user.Department,
                JobTitle = user.JobTitle,
                IsAdmin = perms.Any(p => p.PermissionCode == PermissionCodes.SystemAdmin),
                IsLocked = user.IsLocked,
                Permissions = perms.Select(p => new GetMePermissionsResponse
                {
                    Code = p.PermissionCode,
                    DocumentTypeId = p.DocumentTypeId
                }).ToList()

            };

            return response;
        }
    }
}
