using Application;
using Application.Exceptions;
using Application.Jwt;
using Application.PermissionHandling;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Implementation.UseCases.EntityFramework.Queries.User
{
    public class EFGetMeQuery : EFUseCase, IGetMeQuery
    {
        public int Id => 2;
        public string Name => "User Informations";
        public string Description => "Get user informations";

        private readonly IApplicationActor _actor;
        private readonly IPermissionProvider _permissionProvider;

        public EFGetMeQuery(
            IApplicationActor actor,
            DatabaseContext context,
            IPermissionProvider permissionProvider)
            : base(context)
        {
            _actor = actor;
            _permissionProvider = permissionProvider;
        }

        public async Task<GetMeResponse> ExecuteAsync(EmptySearch search, CancellationToken ct)
        {
            if (_actor is UnauthorizedActor)
                throw new UnauthenticatedException("User is not authenticated.");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == _actor.Id, ct);

            if (user is null)
                throw new EntityNotFoundException("User not found.");

            var perms = await _permissionProvider.GetUserPermissionsAsync(user.Id, ct);

            var isAdmin = perms.Any(p => p.PermissionCode == PermissionCodes.SystemAdmin);

           
            bool canUsers = isAdmin || perms.Any(p => p.PermissionCode == PermissionCodes.UsersRead);
            bool canGroups = isAdmin || perms.Any(p => p.PermissionCode == PermissionCodes.GroupsRead);
            bool canDocumentTypes = isAdmin || perms.Any(p => p.PermissionCode == PermissionCodes.DocumentsRead || p.DocumentTypeId != null);


            var allowedDocTypeIds = perms
                 .Where(p => p.DocumentTypeId.HasValue)
                 .Select(p => p.DocumentTypeId!.Value)
                 .Distinct()
                 .ToList();

            var uiDocumentTypes = canDocumentTypes
                ? await _context.DocumentTypes
                    .AsNoTracking()
                    .Where(dt => isAdmin || allowedDocTypeIds.Contains(dt.Id))
                    .OrderBy(dt => dt.Name)
                    .Select(dt => new GetMeLookupItemResponse
                    {
                        Id = dt.Id,
                        Name = dt.Name
                    })
                    .ToListAsync(ct)
                : new List<GetMeLookupItemResponse>();

            var uiUsers = canUsers
                ? await _context.Users
                .Where(u => u.Id != _actor.Id)
                .AsNoTracking()
                .OrderBy(dt => dt.FirstName)
                .Select(u => new GetMeLookupItemResponse
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                }).ToListAsync(ct) : new List<GetMeLookupItemResponse>();

            var uiGroups = canGroups
                ? await _context.Groups
                    .AsNoTracking()
                    .OrderBy(g => g.Name)
                    .Select(g => new GetMeLookupItemResponse
                    {
                        Id = g.Id,
                        Name = g.Name
                    })
                    .ToListAsync(ct) : new List<GetMeLookupItemResponse>();

            var response = new GetMeResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Department = user.Department,
                JobTitle = user.JobTitle,

                IsAdmin = isAdmin,
                IsLocked = user.IsLocked,

                Permissions = perms.Select(p => new GetMePermissionsResponse
                {
                    Code = p.PermissionCode,
                    DocumentTypeId = p.DocumentTypeId
                }).ToList(),

                Ui = new GetMeUIResponse
                {
                    Can = new GetMeUICanResponse
                    {
                        Users = canUsers,
                        Groups = canGroups,
                        DocumentTypes = canDocumentTypes
                    },
                    DocumentTypes = uiDocumentTypes,
                    Users = uiUsers,
                    Groups = uiGroups
                }
            };

            return response;
        }
    }
}
