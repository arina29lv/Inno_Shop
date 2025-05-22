using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Messaging;

namespace UserControl.Infrastructure.DependencyInjection;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        return services;
    }
}