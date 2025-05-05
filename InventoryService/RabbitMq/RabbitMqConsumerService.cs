
using Common;
using Common.Dtos.Events;
using InventoryService.Data;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InventoryService.RabbitMq
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqConsumerService(IRabbitMqService rabbitMqService,
           IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMqConsumerService> logger)
        {
            _rabbitMqService = rabbitMqService;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMqConsumerService is starting.");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _rabbitMqService.ConsumeMessageFromTopicExchange(exchangeName: Exchanges.OrderExchange,
                 queueName: "orders",
                 routingKey: OrderCreatedEventDto.EventName,
                 ProcessOrderCreatedMessageAsync,
                 cancellationToken
                 );
        }


        private async Task ProcessOrderCreatedMessageAsync(object? sender, BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation requested, skipping message processing.");
                return;
            }

            // Create a scope to resolve the DbContext
            using var scope = _serviceScopeFactory.CreateScope();

            // Resolve the DbContext from the scope
            var orderDbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            // Deserialize the message body to the ProductCreatedEventDto
            string message = Encoding.UTF8.GetString(args.Body.ToArray());

            OrderCreatedEventDto orderCreatedEventDto = JsonSerializer.Deserialize<OrderCreatedEventDto>(message);

            if (orderCreatedEventDto == null)
            {
                _logger.LogError("Order Created Event is null");
                return;
            }


            // Check if the product already exists in the database
            var dbProduct = orderDbContext.Products.SingleOrDefault(x => x.ProductId == orderCreatedEventDto.ProductId);

            // If the product does not exist, add it to the database
            if (dbProduct == null)
            {
                _logger.LogError("Order Created Event product is null");
                return;
            }
            else
            {
                // If the product exists, update its properties

                // If the order amount is greater than the available product quantity, log an error
                if (orderCreatedEventDto.NumberOfItems > dbProduct.Quantity)
                {
                    _logger.LogError(message: "Order Created Event product quantity is not enough");
                    return;
                }

                // Update the product quantity
                dbProduct.Quantity -= orderCreatedEventDto.NumberOfItems;
            }

            // Publish the product updated event to the topic exchange
            await _rabbitMqService.PublishMessageToTopicExchange<ProductUpdatedEventDto>(
                exchangeName: Exchanges.InventoryExchange,
                routingKey: ProductUpdatedEventDto.EventName,
                eventData: new()
                {
                    ProductId = dbProduct.ProductId,
                    Quantity = dbProduct.Quantity,
                });


            // Save the changes to the database
            await orderDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Order Created Message Recieved: {message}");


        }
    }
}
