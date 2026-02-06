using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public sealed class GetDocumentVersionsByDocumentIdResponse
    {

        public Guid DocumentId { get; set; }
        public IReadOnlyList<DocumentVersionDto> Versions { get; set; } = new List<DocumentVersionDto>();

        public class DocumentVersionDto
        {
            public Guid Id { get; set; }
            public int VersionNumber { get; set; }
            public string Note { get; set; } = string.Empty;
            public string AddedBy { get; set; } = string.Empty;
            public bool IsCurrent { get; set; }
            public DateTime CreatedAt { get; set; }

            public IReadOnlyList<VersionFieldDto> Fields { get; set; } = new List<VersionFieldDto>();
        }

        public class VersionFieldDto
        {
            // Definition info
            public int FieldDefinitionId { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public string? Description { get; set; }
            public FieldDataType DataType { get; set; }
            public int SortOrder { get; set; }

            public JsonElement? Value { get; set; }
        }
    }
}
