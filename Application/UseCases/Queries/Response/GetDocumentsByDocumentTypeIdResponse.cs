using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public sealed class GetDocumentsByDocumentTypeIdResponse : PagedResponse<DocumentListItemDto>
    {
    }

    public sealed class DocumentListItemDto
    {
        public Guid DocumentId { get; set; }
        public string Title { get; set; } = string.Empty;

        public int DocumentTypeId { get; set; }
        public int CurrentVersionNumber { get; set; }
        public DateTime VersionCreatedAt { get; set; }


        // Key = FieldDefinition.Code, Value = typed (string/int/decimal/date/bool)
        public Dictionary<string, object?> Fields { get; set; } = new();
    }


    public sealed class DocumentTypeFieldOptionDto
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
