using Application.PermissionHandling.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Requests.Document
{
    public sealed class CreateDocumentRequest : IHasDocumentTypeId
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DocumentTypeId { get; set; }
        public string? Title { get; set; }

        public IReadOnlyCollection<CreateDocumentFieldInputRequest> FieldsInput { get; set; }
            = new List<CreateDocumentFieldInputRequest>();

        public List<DocumentFileInput> Files { get; set; } = new();
    }
    public sealed class CreateDocumentFieldInputRequest
    {
        public int FieldDefinitionId { get; set; }
        public JsonElement Value { get; set; }
    }
}
