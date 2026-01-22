using Application.UseCaseHandling.CQReslover;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCaseHandling.CQResolver
{
    public class QueryResolver : IQueryResolver
    {
        private readonly IServiceProvider _sp;
        public QueryResolver(IServiceProvider sp)
        {
            _sp = sp;
        }
        public IQuery<TSearch, TResponse> Resolve<TSearch, TResponse>()
        {
            return _sp.GetRequiredService<IQuery<TSearch, TResponse>>();
        }
    }
}
