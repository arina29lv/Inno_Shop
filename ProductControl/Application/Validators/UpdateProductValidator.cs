using FluentValidation;
using ProductControl.Application.Command;

namespace ProductControl.Application.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .MaximumLength(30).WithMessage("Name must be no more than 30 characters");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters long")
            .MaximumLength(150).WithMessage("Description must be no more than 150 characters");
        
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required")
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.IsAvailable)
            .NotEmpty().WithMessage("IsAvailable is required");
    }
    
}