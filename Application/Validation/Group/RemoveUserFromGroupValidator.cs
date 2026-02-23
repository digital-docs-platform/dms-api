using Application.UseCases.Commands.Requests.Group;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Group
{
    public class RemoveUserFromGroupValidator : AbstractValidator<RemoveUserFromGroupRequest>
    {
        public RemoveUserFromGroupValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().WithMessage("Group is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User is required.");
        }
    }
}
