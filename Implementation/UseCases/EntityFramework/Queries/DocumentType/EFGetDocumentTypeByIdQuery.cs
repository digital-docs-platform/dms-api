using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework.Queries.DocumentType
{
    public sealed class EFGetDocumentTypeByIdQuery : EFUseCase, IGetDocumentTypeByIdQuery
    {
        public string RequiredPermission => PermissionCodes.DocumentsRead;

        public PermissionScope Scope => PermissionScope.DocumentType;

        public int Id => 7;

        public string Name => "Get document type by id";

        public string Description => "Get document type informations by identifier";


        public EFGetDocumentTypeByIdQuery(DatabaseContext context)
            :base (context)
        {
            
        }

        public async Task<GetDocumentTypeByIdResponse> ExecuteAsync(DocumentTypeIdSearch search, CancellationToken ct)
        {
            var documentType = await _context.DocumentTypes
                .Where(dt => dt.Id == search.DocumentTypeId)
                .Include(dt => dt.FieldDefinitions)
                .FirstOrDefaultAsync(ct);

            if (documentType == null)
                throw new EntityNotFoundException("Document type not found!");

            var AddedBy = await _context.Users.
                Where(u => u.Id == documentType.AddedBy).
                FirstOrDefaultAsync(ct) ??
                throw new EntityNotFoundException("User that added this document type not found? . . .");

            return new GetDocumentTypeByIdResponse
            {
                Id = documentType.Id,
                Name = documentType.Name,
                Description = documentType.Description,
                AddedBy = AddedBy.FirstName + " " + AddedBy.LastName,
                CreatedAt = documentType.CreatedAt,
                ModifiedAt = documentType.ModifiedAt,
                IsDeleted = documentType.IsDeleted,
                DeletedAt = documentType.DeletedAt,
                FieldDefinitions = documentType.FieldDefinitions.Select(fd => new GetDocumentTypeByIdFieldsResponse
                {
                    Id = fd.Id,
                    Label = fd.Label,
                    Code = fd.Code,
                    DataType = fd.DataType,
                    Description = fd.Description,
                    SortOrder = fd.SortOrder,
                    DocumentTypeId = fd.DocumentTypeId,
                    IsRequired = fd.IsRequired,
                    IsSearchable = fd.IsSearchable,
                    IsSortable = fd.IsSortable,
                    CreatedAt = fd.CreatedAt,
                    ModifiedAt = fd.ModifiedAt,
                    IsDeleted = fd.IsDeleted,
                    DeletedAt = fd.DeletedAt
                }).ToList()

            };
        }
    }
}
