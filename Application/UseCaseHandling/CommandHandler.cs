using Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCaseHandling
{
    public class CommandHandler : ICommandHandler
    {
        public async Task HandleAsync<TRequest>(ICommand<TRequest> command, TRequest request, CancellationToken ct)
        {


            await command.ExecuteAsync(request, ct);
        }
    }
}
