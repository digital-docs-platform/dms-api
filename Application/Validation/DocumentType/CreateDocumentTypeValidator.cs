using Application.UseCases.Commands.Requests.DocumentType;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.DocumentType
{
    public class CreateDocumentTypeValidator : AbstractValidator<CreateDocumentTypeRequest>
    {
        public CreateDocumentTypeValidator() 
        {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Name is required.")
             .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
             .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be at most 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Fields)
                .NotNull().WithMessage("Fields collection is required.")
                .Must(f => f.Count > 0).WithMessage("At least one field must be provided.");

            RuleForEach(x => x.Fields).SetValidator(new CreateDocumentTypeFieldsValidator());

            RuleFor(x => x.Fields)
                .Must(fields =>
                {
                    if (fields is null) return true;
                    var labels = fields
                        .Select(f => (f.Label ?? string.Empty).Trim().ToLowerInvariant())
                        .Where(l => !string.IsNullOrWhiteSpace(l));
                    return labels.Distinct().Count() == labels.Count();
                })
                .WithMessage("Field labels must be unique.");

               RuleFor(x => x.Fields)
                .Custom((fields, context) =>
                {
                    if (fields is null) return;

                    var duplicates = fields
                        .GroupBy(f => f.SortOrder)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicates.Count > 0)
                    {
                        context.AddFailure(
                            $"SortOrder must be unique. Duplicates: {string.Join(", ", duplicates)}");
                    }
                });
        }
    }
}
