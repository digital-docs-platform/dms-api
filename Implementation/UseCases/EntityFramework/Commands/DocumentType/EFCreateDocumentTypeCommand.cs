using Application;
using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.DocumentType;
using DataAccess;
using Domain.Entities;
using Implementation.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Commands.DocumentType
{
    public sealed class EFCreateDocumentTypeCommand : EFUseCase,  ICreateDocumentTypeCommand
    {

        public string RequiredPermission => PermissionCodes.SystemAdmin;

        public PermissionScope Scope => PermissionScope.Global;

        public int Id => 3;

        public string Name => "Create DocumentType";

        public string Description => "Create new Document Type";
        
        private readonly IApplicationActor _actor;

        public EFCreateDocumentTypeCommand(IApplicationActor actor, DatabaseContext context) : base(context)
        {
            _actor = actor;
        }

        public async Task ExecuteAsync(CreateDocumentTypeRequest request, CancellationToken ct)
        {

            var exists = await _context.DocumentTypes
                .AnyAsync(x => x.DeletedAt == null && x.Name.ToLower() == request.Name.ToLower(), ct);

            if (exists)
                throw new EntityAlreadyExistsException("Document type with this name already exists.");



            var docType = new Domain.Entities.DocumentType
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                AddedBy = _actor.Id,
                FieldDefinitions = request.Fields.Select(f => new DocumentTypeFieldDefinition
                {
                    Label = f.Label.Trim(),
                    Description = f.Description.Trim(),
                    DataType = f.DataType,
                    SortOrder = f.SortOrder,
                    IsRequired = f.IsRequired,
                    IsSearchable = f.IsSearchable,
                    IsSortable = f.IsSortable,
                    Code = f.Label.Slugify()
                }).ToList()
            };

            await _context.DocumentTypes.AddAsync(docType, ct);
            await _context.SaveChangesAsync(ct);

        }
    }
}
