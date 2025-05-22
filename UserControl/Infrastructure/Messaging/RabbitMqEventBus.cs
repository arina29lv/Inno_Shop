using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Infrastructure.Messaging;

public class RabbitMqEventBus : IEventBus
{
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventBus(IConfiguration config)
    {
        _config = config;

        var host = _config["RabbitMq:Host"] ?? "localhost";
        var port = int.TryParse(_config["RabbitMq:Port"], out var parsedPort) ? parsedPort : 5672;
        var username = _config["RabbitMq:UserName"] ?? "guest";
        var password = _config["RabbitMq:Password"] ?? "guest";

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password,
            DispatchConsumersAsync = true
        };
        
        int retries = 15;
        while (retries-- > 0)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                return;
            }
            catch
            {
                Console.WriteLine("RabbitMQ not ready, retrying...");
                Thread.Sleep(3000);
            }
        }

        throw new Exception("Unable to connect to RabbitMQ");
    }

    private void Publish(string queue, object message)
    {
        _channel.QueueDeclare(
            queue: queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel.BasicPublish(
            exchange: "",
            routingKey: queue,
            basicProperties: null,
            body: body
        );
    }

    public void PublishUserDeactivated(int userId)
    {
        Publish("user.deactivated", new { UserId = userId });
    }

    public void PublishUserActivated(int userId)
    {
        Publish("user.activated", new { UserId = userId });
    }
}