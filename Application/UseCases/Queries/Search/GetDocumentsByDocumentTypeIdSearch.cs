using Application.PermissionHandling.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Search
{
    public sealed class GetDocumentsByDocumentTypeIdSearch : IHasDocumentTypeId
    {
        public int DocumentTypeId { get; set; }

        public PaginationDto Pagination { get; set; } = new();
        public SortDto? Sort { get; set; }

        // Key = FieldDefinition.Code, Value = string | {from,to} | {min,max} ...
        public Dictionary<string, JsonElement>? Filters { get; set; } = new();
    }

    public sealed class PaginationDto
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }

    public sealed class SortDto
    {
        public string Active { get; set; } = string.Empty; // field code OR system field
        public string Direction { get; set; } = "asc";     // "asc" | "desc"
    }
}
