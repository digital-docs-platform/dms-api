using Application.UseCases.Commands.Requests.Permissions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Permissions
{
    public class UpdateGroupPermissionsValidator : AbstractValidator<UpdateGroupPermissionsRequest>
    {
        public UpdateGroupPermissionsValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("Group is not provided.");

            RuleFor(x => x.Permissions)
              .NotNull()
              .WithMessage("Permissions payload is required.");

            RuleForEach(x => x.Permissions)
                .NotNull()
                .WithMessage("Permission item is required.")
                .ChildRules(p =>
                {
                    p.RuleFor(i => i.PermissionId)
                       .NotEmpty().WithMessage("Permission identifier is required.");

                    p.RuleFor(i => i.DocumentTypeId)
                        .Must(id => id is null || id.Value != Guid.Empty)
                        .WithMessage("DocumentTypeId is invalid.");
                });
        }
    }
}
