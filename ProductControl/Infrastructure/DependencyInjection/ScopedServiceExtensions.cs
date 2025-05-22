using ProductControl.Domain.Interfaces;
using ProductControl.Infrastructure.Repositories;

namespace ProductControl.Infrastructure.DependencyInjection;

public static class ScopedServiceExtensions
{
    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}