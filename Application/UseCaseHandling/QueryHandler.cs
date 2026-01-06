using Application.PermissionHandling;
using Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.UseCaseHandling
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPermissionHandler _permissionHandler;
        public QueryHandler(IPermissionHandler permissionHandler)
        {
            _permissionHandler = permissionHandler;
        }
        public async Task<TResponse> HandleAsync<TSearch, TResponse>(IQuery<TSearch, TResponse> query, TSearch search, CancellationToken ct) where TResponse : class
        {
            if (query is IProtectedUseCase protectedUseCase)
            {
                _permissionHandler.EnsureAll(protectedUseCase.RequiredPermissions);
            }


            return await query.ExecuteAsync(search, ct);
        }
    }
}
