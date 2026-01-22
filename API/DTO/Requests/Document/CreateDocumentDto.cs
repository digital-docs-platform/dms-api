using System.Text.Json;

namespace API.DTO.Requests.Document
{
    public class CreateDocumentDto
    {
        public string? Title { get; set; }
        public int DocumentTypeId { get; set; }
        public IReadOnlyCollection<CreateDocumentFieldInputDto> FieldsInput { get; set; } 
            = new List<CreateDocumentFieldInputDto>();
    }

    public class CreateDocumentFieldInputDto
    {
        public int FieldDefinitionId { get; set; }
        public JsonElement Value { get; set; }
    }
}
