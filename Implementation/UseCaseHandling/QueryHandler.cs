using Application.PermissionHandling;
using Application.PermissionHandling.Resolver;
using Application.UseCaseHandling;
using Application.UseCaseHandling.CQReslover;
using Application.UseCases;
using Application.Validation;
using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Implementation.UseCaseHandling
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPermissionHandler _permissionHandler;
        private readonly IDocumentTypeResolver _documentTypeResolver;
        private readonly IRequestValidation _requestValidation;


        public QueryHandler(
            IPermissionHandler permissionHandler,
            IDocumentTypeResolver documentTypeResolver,
            IRequestValidation requestValidation)
        {
            _permissionHandler = permissionHandler;
            _documentTypeResolver = documentTypeResolver;
            _requestValidation = requestValidation;

        }
        public async Task<TResponse> HandleAsync<TSearch, TResponse>(IQuery<TSearch, TResponse> query, TSearch search, CancellationToken ct) where TResponse : class
        {


            if (query is IProtectedUseCase protectedUseCase)
            {
                if (protectedUseCase.Scope == PermissionScope.DocumentType)
                {
                    Guid docType = search != null ? await _documentTypeResolver.ResolveAsync(search, ct) : throw new Exception();
                    await _permissionHandler.EnsureAsync(protectedUseCase.RequiredPermission, docType, ct);
                }
                else
                {
                    await _permissionHandler.EnsureAsync(protectedUseCase.RequiredPermission, documentTypeId: null, ct);
                }
            }

            await _requestValidation.ValidateAsync(search, ct);


            return await query.ExecuteAsync(search, ct);
        }
    }
}
