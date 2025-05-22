using FluentValidation;
using UserControl.Application.Commands.UserCommands;

namespace UserControl.Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required.")
            .GreaterThan(0).WithMessage("ID must be greater than 0.");
            
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be invalid.");
        
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => new[] { "admin", "manager", "user" }.Contains(role.ToLower())).WithMessage("Role must be a valid role: admin, manager, user.");
    }
}