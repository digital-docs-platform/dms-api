using Application;
using Application.Exceptions;
using Application.Jwt;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.User
{
    public class EFGetUserByIdQuery : EFUseCase, IGetUserByIdQuery
    {
        public string RequiredPermission => PermissionCodes.UsersRead;

        public PermissionScope Scope => PermissionScope.Global;

        public int Id => 5;

        public string Name => "Get_user_by_id";

        public string Description => "Get user by ID";


        private readonly IApplicationActor _actor;
        public EFGetUserByIdQuery(IApplicationActor actor, DatabaseContext context)
            : base(context)
        { 
            _actor = actor;
        }

        public async Task<GetUserByIdResponse> ExecuteAsync(IdSearch search, CancellationToken ct)
        {

            if (_actor is UnauthorizedActor)
                throw new UnauthenticatedException("User is not autheticated!");

            var user = await _context.Users.FindAsync(search.Id);

            if (user == null)
                throw new EntityNotFoundException($"There is no user with id: {search.Id}");

            return new GetUserByIdResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Department = user.Department,
                JobTitle = user.JobTitle
            };

        }
    }
}
