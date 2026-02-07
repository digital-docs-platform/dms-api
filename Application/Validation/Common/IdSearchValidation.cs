using Application.UseCases.Queries.Search;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Common
{
    public sealed class IdSearchValidation : AbstractValidator<IdSearch>
    {
        public IdSearchValidation() 
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Identifier need to be provided");
        }
    }
}
