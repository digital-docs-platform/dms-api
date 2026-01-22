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
        public int DocumentTypeId { get; set; }
        public string? Title { get; set; }

        public IReadOnlyCollection<CreateDocumentFieldInputRequest> FieldsInput { get; set; }
            = new List<CreateDocumentFieldInputRequest>();
    }
    public sealed class CreateDocumentFieldInputRequest
    {
        public int FieldDefinitionId { get; set; }
        public JsonElement Value { get; set; }
    }
}
