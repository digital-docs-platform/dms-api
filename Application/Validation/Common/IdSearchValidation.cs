using Application.UseCases.Queries.Search;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Common
{
    public sealed class IdSearchValidation<TType>
        : AbstractValidator<IdSearch<TType>>
    {
        public IdSearchValidation()
        {
            RuleFor(x => x.Id)
             .NotEqual(default(TType))
             .WithMessage("Identifier must be provided");
        }
    }
}
