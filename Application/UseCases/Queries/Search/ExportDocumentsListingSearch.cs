using Application.PermissionHandling.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Search
{
    public sealed class ExportDocumentsListingSearch : IHasDocumentTypeId
    {
        public Guid DocumentTypeId { get; set; }
        public string Title { get; set; } = "Documents Listing . . .";
        public Dictionary<string, JsonElement>? Filters { get; set; }
        public SortExportDto? Sort {  get; set; }
        public List<string> Columns { get; set; } = new();
        public ListingExportFormat Format { get; set; }
    }

    public sealed class SortExportDto
    {
        public string Active { get; set; } = string.Empty; 
        public string Direction { get; set; } = "asc";     
    }
}
