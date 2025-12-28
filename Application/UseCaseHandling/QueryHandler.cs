using Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCaseHandling
{
    public class QueryHandler : IQueryHandler
    {
        public async Task<TResponse> HandleAsync<TSearch, TResponse>(IQuery<TSearch, TResponse> query, TSearch search, CancellationToken ct) where TResponse : class
        {

            return await query.ExecuteAsync(search, ct);
        }
    }
}
