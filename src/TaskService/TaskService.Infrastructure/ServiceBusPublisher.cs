using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskService.Application.Interfaces;

namespace TaskService.Infrastructure
{
    public class ServiceBusPublisher: IEventPublisher
    {
        private readonly ServiceBusSender _sender;
        private readonly ILogger<ServiceBusPublisher> _logger;

        public ServiceBusPublisher(IConfiguration config, ILogger<ServiceBusPublisher> logger)
        {
            _logger = logger;

            _logger.LogInformation("Environment: {Env}",
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

            _logger.LogInformation("SB Conn from config: {Value}",
                config["ServiceBus:ConnectionString"]);

            _logger.LogInformation("SB Conn from ENV directly: {Value}",
                Environment.GetEnvironmentVariable("ServiceBus__ConnectionString"));


            var connectionString = config["ServiceBus:ConnectionString"];
            var queueName = config["ServiceBus:QueueName"];

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Service Bus connection string missing");

            var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                RetryOptions = new ServiceBusRetryOptions
                {
                    Mode = ServiceBusRetryMode.Exponential,
                    MaxRetries = 5,
                    Delay = TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(10)
                }
            });
            _sender = client.CreateSender(queueName);
        }

        public async Task PublishAsync<T>(T message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var busMessage = new ServiceBusMessage(json);

                await _sender.SendMessageAsync(busMessage);

                _logger.LogInformation("Event published to Service Bus: {EventType}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to publish event {EventType} to Service Bus",
                    typeof(T).Name);

                throw;
            }
        }
    }
}
