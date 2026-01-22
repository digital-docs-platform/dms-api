using Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCaseHandling
{
    public interface ICommandHandler
    {
        public Task HandleAsync<TRequest>(ICommand<TRequest> command,TRequest request, CancellationToken ct);
    }
}
