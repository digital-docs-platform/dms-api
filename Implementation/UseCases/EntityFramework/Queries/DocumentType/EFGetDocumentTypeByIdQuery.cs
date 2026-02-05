using Application.Exceptions;
using Application.PermissionHandling;
using Application.UseCases;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Response;
using Application.UseCases.Queries.Search;
using DataAccess;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

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
            : base(context)
        {
        }

        public async Task<GetDocumentTypeByIdResponse> ExecuteAsync(DocumentTypeIdSearch search, CancellationToken ct)
        {
            var documentType = await _context.DocumentTypes
                .Where(dt => dt.Id == search.DocumentTypeId)
                .Include(dt => dt.FieldDefinitions)
                    .ThenInclude(fd => fd.Options) // NEW
                .FirstOrDefaultAsync(ct);

            if (documentType == null)
                throw new EntityNotFoundException("Document type not found!");

            var addedByUser = await _context.Users
                .Where(u => u.Id == documentType.AddedBy)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync(ct);

            if (addedByUser is null)
                throw new EntityNotFoundException("User that added this document type not found? . . .");

            return new GetDocumentTypeByIdResponse
            {
                Id = documentType.Id,
                Name = documentType.Name,
                Description = documentType.Description,
                AddedBy = addedByUser.FirstName + " " + addedByUser.LastName,
                CreatedAt = documentType.CreatedAt,
                ModifiedAt = documentType.ModifiedAt,
                IsDeleted = documentType.IsDeleted,
                DeletedAt = documentType.DeletedAt,

                FieldDefinitions = documentType.FieldDefinitions
                    .Select(fd => new GetDocumentTypeByIdFieldsResponse
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
                        DeletedAt = fd.DeletedAt,

                        //  only for Select, otherwise empty list
                        Options = fd.DataType == FieldDataType.Select
                            ? fd.Options
                                .Where(o => !o.IsDeleted)
                                .OrderBy(o => o.SortOrder)
                                .Select(o => new GetDocumentTypeFieldOptionResponse
                                {
                                    Id = o.Id,
                                    FieldDefinitionId = o.FieldDefinitionId,
                                    Value = o.Value,
                                    Label = o.Label,
                                    SortOrder = o.SortOrder
                                })
                                .ToList()
                            : new List<GetDocumentTypeFieldOptionResponse>()
                    })
                    .OrderBy(fd => fd.SortOrder)
                    .ToList()
            };
        }
    }
}
