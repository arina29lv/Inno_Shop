using FluentValidation.AspNetCore;

namespace UserControl.Infrastructure.DependencyInjection;

public static class ValidationServiceExtensions
{
    public static IServiceCollection AddValidationService(this IServiceCollection services)
    {
        services.AddControllers().AddFluentValidation(fv => 
        { 
            fv.RegisterValidatorsFromAssemblyContaining<Program>(); 
        });
        
        return services;
    }
}