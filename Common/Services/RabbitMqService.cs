using Common.Events.Dtos;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Common.Services
{
    internal class RabbitMqService : IMessageQueueService
    {
        private readonly ILogger<RabbitMqService> _logger;
        private IConnection? _connection;
        private readonly IModel _channel;

        public RabbitMqService(ILogger<RabbitMqService> logger,
        IConnection messageConnection)
        {
            _logger = logger;
            _connection = messageConnection;

            // Create a persistent channel
            _channel = _connection.CreateModel();
        }
        public async Task PublishMessageToTopicExchange<T>(string exchangeName, string routingKey, T eventData) where T : EventDto
        {
            // Create a topic exchange
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventData));

            // Publish the message to the topic exchange with a routing key
            _channel.BasicPublish(exchangeName, routingKey, basicProperties: null, body);

            // Log the sent message
            _logger.LogInformation("Exchange {ExchangeName} published event {EventData} with routing key {RoutingKey}", exchangeName, eventData, routingKey);

            await Task.CompletedTask;
        }

        public async Task ConsumeMessageFromTopicExchange(
            string exchangeName,
            string queueName,
            string routingKey,
            Func<object?, BasicDeliverEventArgs, CancellationToken, Task> processMessage,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Cancellation requested, message consumption.");
                return;
            }

            // Create a topic exchange
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            // Declare a queue and bind it to the topic exchange with a routing key
            var queue = _channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false);
            _channel.QueueBind(queueName, exchangeName, routingKey: routingKey);

            // Create a consumer and start consuming messages
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // Handle the received messages
            consumer.Received += async (model, ea) =>
            {   
                _logger.LogInformation("Message received in queue: {QueueName}", queueName);
                await processMessage(model, ea, cancellationToken);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            // Start consuming messages from the queue
            _channel.BasicConsume(queueName, autoAck: false, consumer);

            await Task.CompletedTask;

        }
    }
}
