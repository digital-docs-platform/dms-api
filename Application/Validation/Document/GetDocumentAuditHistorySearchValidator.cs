using Application.UseCases.Queries.Search;
using FluentValidation;

namespace Application.Validation.Document
{
    public sealed class GetDocumentAuditHistorySearchValidator : AbstractValidator<GetDocumentAuditHistorySearch>
    {
        public GetDocumentAuditHistorySearchValidator()
        {
            RuleFor(x => x.DocumentId)
                .NotEmpty()
                .WithMessage("DocumentId must be provided.");

            RuleForEach(x => x.EventTypes)
                .IsInEnum()
                .WithMessage("One or more provided event types are invalid.");

            When(x => x.Sort != null, () =>
            {
                RuleFor(x => x.Sort.Direction)
                    .Must(d => d.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            || d.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Sort.Direction must be 'asc' or 'desc'.");

                RuleFor(x => x.Sort.Active)
                    .NotEmpty()
                    .MaximumLength(100);
            });
        }
    }
}
