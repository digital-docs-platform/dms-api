using Application.UseCases.Commands.Requests.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.User
{
    public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator() 
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
            RuleFor(x => x.JobTitle).MaximumLength(150);
            RuleFor(x => x.Department).MaximumLength(150);
        }
    }
}
