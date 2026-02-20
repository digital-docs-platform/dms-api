using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.User
{
    public sealed class EFGetUserGrantPermissionsQuery : EFUseCase, IGetUserGrantPermissionsQuery
    {
        public string RequiredPermission => PermissionCodes.SystemAdmin;

        public PermissionScope Scope => PermissionScope.System;

        public int Id => 15;

        public string Name => "Get user grant permissions";

        public string Description => "Get only user grant permissions ";


        private readonly IPermissionProvider _permissionProvider;
        public EFGetUserGrantPermissionsQuery(IPermissionProvider permissionProvider, DatabaseContext context)
            : base(context)
        {
            _permissionProvider = permissionProvider;
        }

        public async Task<GetUserPermissionsResponse> ExecuteAsync(IdSearch<Guid> search, CancellationToken ct)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == search.Id, ct))
                throw new EntityNotFoundException("User not found");

            var userPermissions = await _permissionProvider.GetUserPermissionGrantsAsync(search.Id, ct);

            return new GetUserPermissionsResponse
            {
                Permissions = userPermissions.Select(up => new PermissionsDto
                {
                    Code = up.PermissionCode,
                    DocumentTypeId = up.DocumentTypeId
                }).ToList()
            };


        }
    }
}
