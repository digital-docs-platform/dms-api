using Application;
using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.Document;
using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Commands.Document
{
    public class EFCreateDocumentCommand : EFUseCase, ICreateDocumentCommand
    {
        public string RequiredPermission => PermissionCodes.DocumentsWrite;

        public PermissionScope Scope => PermissionScope.DocumentType;

        public int Id => 4;

        public string Name => "Create new document";

        public string Description => "Create new instance that represents a document (not new DocumentVersion)";


        private readonly IApplicationActor _actor;
        public EFCreateDocumentCommand(DatabaseContext context, IApplicationActor actor) : base(context)
        {
            _actor = actor;
        }

        public async Task ExecuteAsync(CreateDocumentRequest request, CancellationToken ct)
        {
            
            var defs = await _context.DocumentTypeFieldDefinitions
                .Where(d => d.DocumentTypeId == request.DocumentTypeId && (d.DeletedAt == null && d.IsDeleted == false))
                .ToListAsync(ct);

            var defsById = defs.ToDictionary(d => d.Id);

            
            var inputsById = request.FieldsInput.ToDictionary(f => f.FieldDefinitionId);

            //foreach (var input in request.FieldsInput)
            //    if (!defsById.ContainsKey(input.FieldDefinitionId))
            ////        throw new RequestDataValidationException(new[]
            ////        {
            ////    $"FieldDefinitionId={input.FieldDefinitionId} does not belong to DocumentTypeId={request.DocumentTypeId}."
            ////});

            //var missingRequired = defs
            //    .Where(d => d.IsRequired)
            //    .Where(d => !inputsById.TryGetValue(d.Id, out var inp) || IsNullOrEmpty(inp.Value))
            //    .Select(d => d.Code ?? d.Id.ToString())
            //    .ToList();

            //if (missingRequired.Count > 0)
            //    throw new RequestDataValidationException(missingRequired.Select(x => $"Required field missing: {x}").ToList());

            
            var version = new DocumentVersion
            {
                VersionNumber = 1,
                CreatedBy = _actor.Id,
                FieldValues = request.FieldsInput.Select(input =>
                {
                    var def = defsById[input.FieldDefinitionId];

                    var fv = new DocumentTypeFieldValue
                    {
                        FieldDefinitionId = def.Id,
                        
                    };

                    //ApplyTypedValue(def.DataType, input.Value, fv);
                    return fv;
                }).ToList()
            };

            var doc = new Domain.Entities.Document
            {
                DocumentTypeId = request.DocumentTypeId,
                Title = request.Title.Trim(),
                CreatedBy = _actor.Id,
                DocumentVersions = new List<DocumentVersion> { version }
            };


            _context.Documents.Add(doc);
            await _context.SaveChangesAsync(ct);

        }
    }
}
