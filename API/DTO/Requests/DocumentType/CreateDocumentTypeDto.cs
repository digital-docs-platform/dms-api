using Domain.Enums;
using System.Text.Json.Serialization;

namespace API.DTO.Requests.DocumentType
{
    public sealed class CreateDocumentTypeDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyCollection<CreateDocumentTypeFieldDto> Fields { get; set; } = new List<CreateDocumentTypeFieldDto>();
    }

    public sealed class CreateDocumentTypeFieldDto
    {
        public string Label { get; set; }
        public string Description { get; set; }

       
        public FieldDataType DataType { get; set; }

        public int SortOrder { get; set; }
        public bool IsRequired { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsSortable { get; set; }
    }
}
