using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastructure.Persistence;

namespace ProductControl.Infrastructure.DependencyInjection;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}