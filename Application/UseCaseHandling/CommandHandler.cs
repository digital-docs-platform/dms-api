using Application.PermissionHandling;
using Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCaseHandling
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IPermissionHandler _permissionHandler;
        public CommandHandler(IPermissionHandler permissionHandler) 
        {
            _permissionHandler = permissionHandler;
        }
        public async Task HandleAsync<TRequest>(ICommand<TRequest> command, TRequest request, CancellationToken ct)
        {
            if(command is IProtectedUseCase protectedUseCase)
            {
                _permissionHandler.EnsureAll(protectedUseCase.RequiredPermissions);
            }



            await command.ExecuteAsync(request, ct);
        }
    }
}
