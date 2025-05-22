using ProductControl.Infrastructure.Messaging;

namespace ProductControl.Infrastructure.DependencyInjection;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
        services.AddHostedService<RabbitMqListener>();
        return services;
    }
}