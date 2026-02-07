using Application.UseCases.Commands.Requests.Document;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Application.Validation.Document
{
    public sealed class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequest>
    {
        public CreateDocumentRequestValidator()
        {
            RuleFor(x => x.DocumentTypeId)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.FieldsInput)
                .NotNull();

            RuleForEach(x => x.FieldsInput).ChildRules(item =>
            {
                item.RuleFor(f => f.FieldDefinitionId)
                    .GreaterThan(0);

                // Value je JsonElement (struct) -> ne moze null, ali moze Undefined/Null
                item.RuleFor(f => f.Value)
                    .Must(v => v.ValueKind != JsonValueKind.Undefined)
                    .WithMessage("Value is missing.");
            });

            // Ne dozvoli duplikate FieldDefinitionId u payloadu
            RuleFor(x => x.FieldsInput)
                .Must(NoDuplicateFieldDefinitionIds)
                .WithMessage("FieldsInput contains duplicate FieldDefinitionId values.");
        }

        private static bool NoDuplicateFieldDefinitionIds(IReadOnlyCollection<CreateDocumentFieldInputRequest> inputs)
        {
            if (inputs is null || inputs.Count == 0) return true;

            return inputs
                .GroupBy(x => x.FieldDefinitionId)
                .All(g => g.Count() == 1);
        }
    }
}
