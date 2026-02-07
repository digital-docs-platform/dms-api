using Application.PermissionHandling;
using Application.PermissionHandling.Resolver;
using Application.UseCaseHandling;
using Application.UseCaseHandling.CQReslover;
using Application.UseCases;
using Application.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCaseHandling
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IPermissionHandler _permissionHandler;
        private readonly IDocumentTypeResolver _documentTypeResolver;
        private readonly IRequestValidation _requestValidation;


        public CommandHandler(
            IPermissionHandler permissionHandler, 
            IDocumentTypeResolver documentTypeResolver, 
            IRequestValidation requestValidation) 
        {
            _permissionHandler = permissionHandler;
            _documentTypeResolver = documentTypeResolver;
            _requestValidation = requestValidation;
        }

        public async Task HandleAsync<TRequest>(ICommand<TRequest> command, TRequest request, CancellationToken ct)
        {

            if(command is IProtectedUseCase protectedUseCase)
            {
                if (protectedUseCase.Scope == PermissionScope.DocumentType)
                {
                    Guid docType = await _documentTypeResolver.ResolveAsync(request, ct);
                    await _permissionHandler.EnsureAsync(protectedUseCase.RequiredPermission, docType, ct);
                }
                else
                {
                    await _permissionHandler.EnsureAsync(protectedUseCase.RequiredPermission, documentTypeId: null, ct);
                }
            }

            await _requestValidation.ValidateAsync(request, ct);

            await command.ExecuteAsync(request, ct);
        }
    }
}
