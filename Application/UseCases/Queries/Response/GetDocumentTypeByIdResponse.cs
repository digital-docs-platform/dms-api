using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.UseCases.Queries.Response
{
    public class GetDocumentTypeByIdResponse
    {
        public Guid Id { get; set; }

        // osnovne info o tipu
        public string Name { get; set; }
        public string Description { get; set; }

        // audit / meta (korisno za admin UI)
        public string AddedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // soft-delete info (front može da sakrije/disable, ili da pokaže status)
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // fields
        public List<GetDocumentTypeByIdFieldsResponse> FieldDefinitions { get; set; } = new();
    }

    public class GetDocumentTypeByIdFieldsResponse
    {
        public int Id { get; set; }
        public Guid DocumentTypeId { get; set; }

        // bitno za render + validaciju + mapiranje payload-a
        public string Code { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public FieldDataType DataType { get; set; }

        // UI pravila
        public bool IsRequired { get; set; }
        public int SortOrder { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsSortable { get; set; }

        // meta (opciono, ali korisno za admin ekran)
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // soft-delete (ako želiš da prikazuješ “archived” polja)
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<GetDocumentTypeFieldOptionResponse> Options { get; set; } = new();
    }


    public class GetDocumentTypeFieldOptionResponse
    {
        public int Id { get; set; }
        public int FieldDefinitionId { get; set; }

        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int SortOrder { get; set; }

    }
}
