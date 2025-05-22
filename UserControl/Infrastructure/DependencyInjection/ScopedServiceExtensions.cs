using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Repositories;
using UserControl.Infrastructure.Services;

namespace UserControl.Infrastructure.DependencyInjection;

public static class ScopedServiceExtensions
{
    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}