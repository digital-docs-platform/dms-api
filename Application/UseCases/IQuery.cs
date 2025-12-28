using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IQuery<TSearch, TResponse> : IUseCase
    {
        public Task<TResponse> ExecuteAsync(TSearch search, CancellationToken ct);
    }
}
