using Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCaseHandling
{
    public interface IQueryHandler
    {
        public Task<TResponse> HandleAsync<TSearch, TResponse>(IQuery<TSearch, TResponse> query, TSearch search, CancellationToken ct)
            where TResponse : class;
    }
}
