using System.Reflection;
using MediatR;
using ProductControl.Application.Mappings;

namespace ProductControl.Infrastructure.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }
}