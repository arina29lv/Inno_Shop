using Microsoft.EntityFrameworkCore;
using UserControl.Infrastructure.Persistence;

namespace UserControl.Infrastructure.DependencyInjection;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}