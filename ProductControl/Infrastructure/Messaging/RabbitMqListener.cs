using System.Text;
using System.Text.Json;
using MediatR;
using ProductControl.Application.Command;
using ProductControl.Application.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductControl.Infrastructure.Messaging;

public class RabbitMqListener : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;

    public RabbitMqListener(IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMq:Host"] ?? "localhost",
            Port = int.TryParse(_config["RabbitMq:Port"], out var port) ? port : 5672,
            UserName = _config["RabbitMq:UserName"] ?? "guest",
            Password = _config["RabbitMq:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };
        
        IConnection? connection = null;
        int retries = 15;

        while (retries-- > 0)
        {
            try
            {
                connection = factory.CreateConnection();
                break;
            }
            catch
            {
                Console.WriteLine("RabbitMQ not ready, retrying...");
                Thread.Sleep(3000);
            }
        }

        if (connection == null)
        {
            throw new Exception("RabbitMQ is not reachable after several attempts.");
        }
        
        var channel = connection.CreateModel();

        channel.QueueDeclare("user.deactivated", false, false, false, null);
        channel.QueueDeclare("user.activated", false, false, false, null);

        var deactivateConsumer = new AsyncEventingBasicConsumer(channel);
        deactivateConsumer.Received += async (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var data = JsonSerializer.Deserialize<UserEventDto>(message);

            if (data == null) return;

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new DeactivateProductsCommand {UserId = data.UserId});
        };

        var activateConsumer = new AsyncEventingBasicConsumer(channel);
        activateConsumer.Received += async (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var data = JsonSerializer.Deserialize<UserEventDto>(message);

            if (data == null) return;

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new ActivateProductsCommand { UserId = data.UserId});
        };

        channel.BasicConsume("user.deactivated", true, deactivateConsumer);
        channel.BasicConsume("user.activated", true, activateConsumer);

        return Task.CompletedTask;
    }
}