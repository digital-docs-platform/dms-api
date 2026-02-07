using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public sealed class GetDocumentByIdResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public Guid DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;

        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public LatestDocumentVersionDto? LatestVersion { get; set; }
    }

    public sealed class LatestDocumentVersionDto
    {
        public Guid Id { get; set; }

        // Ako imaš VersionNumber na DocumentVersion, koristi ga.
        public int VersionNumber { get; set; }

        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<DocumentFieldValueDto> Fields { get; set; } = new();
    }

    public sealed class DocumentFieldValueDto
    {
        public int FieldDefinitionId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public FieldDataType DataType { get; set; }

        // typed value: string/int/decimal/DateTime ili za select: OptionId/OptionLabel (ispod)
        public object? Value { get; set; }

        public int? OptionId { get; set; }
        public string? OptionLabel { get; set; }
    }
}
