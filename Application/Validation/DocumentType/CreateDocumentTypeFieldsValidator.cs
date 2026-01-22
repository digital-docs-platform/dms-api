using Application.UseCases.Commands.Requests.DocumentType;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.DocumentType
{
    public sealed class CreateDocumentTypeFieldsValidator : AbstractValidator<CreateDocumentTypeFieldRequest>
    {
        public CreateDocumentTypeFieldsValidator()
        {
            RuleFor(x => x.Label)
               .NotEmpty().WithMessage("Field label is required.")
               .MaximumLength(100).WithMessage("Field label must be at most 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("Field description must be at most 300 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.DataType)
                .IsInEnum().WithMessage("Invalid field data type.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("SortOrder must be 0 or greater.");

            
        }
    }
}
