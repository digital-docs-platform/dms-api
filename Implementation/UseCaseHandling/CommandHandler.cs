using Application;
using Application.Exceptions;
using Application.Logging;
using Application.PermissionHandling;
using Application.PermissionHandling.Resolver;
using Application.UseCaseHandling;
using Application.UseCaseHandling.CQReslover;
using Application.UseCases;
using Application.Validation;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private readonly IAuditLogger _auditLogger;
        private readonly IApplicationActor _actor;
        private readonly IRequestContext _requestContext;


        public CommandHandler(
            IPermissionHandler permissionHandler,
            IDocumentTypeResolver documentTypeResolver,
            IRequestValidation requestValidation,
            IAuditLogger auditLogger,
            IApplicationActor actor,
            IRequestContext requestContext)
        {
            _permissionHandler = permissionHandler;
            _documentTypeResolver = documentTypeResolver;
            _requestValidation = requestValidation;
            _auditLogger = auditLogger;
            _actor = actor;
            _requestContext = requestContext;
        }

        public async Task HandleAsync<TRequest>(ICommand<TRequest> command, TRequest request, CancellationToken ct)
        {

            if(command is IProtectedUseCase protectedUseCase)
            {
                if (protectedUseCase.Scope == PermissionScope.Document)
                {
                    Guid docType = request != null ? await _documentTypeResolver.ResolveAsync(request, ct) : throw new Exception();
                    await _permissionHandler.EnsureAsync(protectedUseCase.RequiredPermission, docType, ct);
                }
                else
                {
                    await _permissionHandler.EnsureAsync(protectedUseCase.RequiredPermission, documentTypeId: null, ct);
                }
            }

            await _requestValidation.ValidateAsync(request, ct);

            await command.ExecuteAsync(request, ct);

            if (command is IAuditableUseCase<TRequest> auditable)
            {
                var entry = auditable.BuildAuditEntry(request, _actor);
                entry.IpAddress = _requestContext.IpAddress;
                await _auditLogger.LogAsync(entry, ct);
            }
        }
    }
}
