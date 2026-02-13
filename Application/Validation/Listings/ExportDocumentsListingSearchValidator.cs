using Application.UseCases.Queries.Search;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Listings
{
    public sealed class ExportDocumentsListingSearchValidator : AbstractValidator<ExportDocumentsListingSearch>
    {
        public ExportDocumentsListingSearchValidator()
        {
            //RuleFor(x => x.DocumentTypeId)
            //    .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Format)
                .IsInEnum();

            RuleFor(x => x.Columns)
                .NotNull()
                .Must(c => c.Count > 0)
                .WithMessage("You must select at least one column.")
                .Must(c => c.Count <= 30)
                .WithMessage("You can export maximum 30 columns.");

            RuleForEach(x => x.Columns)
                .NotEmpty()
                .MaximumLength(100);

            When(x => x.Sort != null, () =>
            {
                RuleFor(x => x.Sort!.Direction)
                    .Must(d => d.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            || d.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Sort.Direction must be 'asc' or 'desc'.");

                RuleFor(x => x.Sort!.Active)
                    .NotEmpty()
                    .MaximumLength(100);
            });
        }
    }
}
