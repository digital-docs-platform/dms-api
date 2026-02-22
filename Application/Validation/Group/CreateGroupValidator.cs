using Application.UseCases.Commands.Requests.Group;
using FluentValidation;


namespace Application.Validation.Group
{
    public class CreateGroupValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Group name is required.")
                .MaximumLength(100)
                .WithMessage("Maximum lenght of name is 100 char");

            RuleFor(x => x.Description)
                .MaximumLength(300)
                .WithMessage("Maximum lenght of description is 300 char");

            RuleForEach(x => x.Permissions)
                .ChildRules(x =>
                {
                    x.RuleFor(p => p.Code)
                    .NotEmpty()
                    .WithMessage("Permission Code is required")
                    .MaximumLength(300)
                    .WithMessage("Max number of characters for Permission code is 300");

                    x.RuleFor(p => p.DocumentTypeId)
                        .Must(id => id is null || id.Value != Guid.Empty)
                        .WithMessage("DocumentTypeId is invalid.");
                });
        }
    }
}
