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
    public sealed class EFGetGroupPermissionsQuery : EFUseCase, IGetGroupPermissionsQuery
    {
        public string RequiredPermission => PermissionCodes.SystemAdmin;

        public PermissionScope Scope => PermissionScope.System;

        public int Id => 14;

        public string Name => "Get Group Permissions";

        public string Description => "Get only group granted permissions";



        private readonly IPermissionProvider _permissionProvider;
        public EFGetGroupPermissionsQuery(IPermissionProvider permissionProvider, DatabaseContext context)
            : base(context)
        {
            _permissionProvider = permissionProvider;
        }
        public async Task<GetGroupPermissionsResponse> ExecuteAsync(IdSearch<Guid> search, CancellationToken ct)
        {
            if (!await _context.Groups.AnyAsync(g => g.Id == search.Id, ct))
                throw new EntityNotFoundException("Group not found.");

            var groupPermissions = await _permissionProvider.GetGroupPermissionGrantsAsync(search.Id, ct);

            return new GetGroupPermissionsResponse 
            {
                Permissions = groupPermissions.Select(up => new PermissionsDto
                {
                    Code = up.PermissionCode,
                    DocumentTypeId = up.DocumentTypeId
                }).ToList()
            };
        }
    }
}
