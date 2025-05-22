using FluentValidation;
using UserControl.Application.Commands;
using UserControl.Application.Commands.AuthCommands;

namespace UserControl.Application.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
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