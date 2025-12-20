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
        public TResult Handle<TSearch, TResult>(IQuery<TSearch, TResult> query, TSearch search) where TResult : class
        {
            throw new NotImplementedException();
        }
    }
}
