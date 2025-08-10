using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class RabbitMQNotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQNotificationService> _logger;
    private readonly string _exchangeName = "ecommerce.notifications";

    public RabbitMQNotificationService(IConfiguration configuration, ILogger<RabbitMQNotificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendNotificationAsync(NotificationEvent notificationEvent)
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost",
                Port = _configuration.GetValue<int>("RabbitMQ:Port", 5672),
                UserName = _configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest",
                Password = _configuration.GetValue<string>("RabbitMQ:Password") ?? "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);

            var message = JsonSerializer.Serialize(notificationEvent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: "order.created",
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Notification sent: {EventName}", notificationEvent.EventName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification: {EventName}", notificationEvent.EventName);
        }
    }
}