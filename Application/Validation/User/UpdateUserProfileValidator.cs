using Application.UseCases.Commands.Requests.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.User
{
    public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileRequest>
    {
        public UpdateUserProfileValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Provide User");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name need to be provided!")
                .MaximumLength(100).WithMessage("Max length for First Name is: 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name need to be provided!")
                .MaximumLength(100).WithMessage("Max length for Last Name is: 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email need to be provided!")
                .EmailAddress().WithMessage("Provide valid email address!")
                .MaximumLength(320).WithMessage("Maximum length of Email is: 320 characters");

            RuleFor(x => x.JobTitle)
                .MaximumLength(150).WithMessage("Maximum length of Department is: 150 characters");

            RuleFor(x => x.Department)
                .MaximumLength(200).WithMessage("Maximum length of Department is: 200 characters");


        }
    }
}
