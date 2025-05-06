
using Common;
using Common.Events;
using Common.Events.Dtos;
using Common.Services;
using OrderService.Data;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderService.RabbitMq
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private readonly IMessageQueueService _rabbitMqService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqConsumerService(IMessageQueueService rabbitMqService,
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
            await new ProductCreatedEvent(null, _rabbitMqService).ConsumeAsync(queueName: "inventory-product", ProcessProductCreatedMessageAsync, cancellationToken);
            await new ProductCreatedEvent(null, _rabbitMqService).ConsumeAsync(queueName: "inventory-product", ProcessProductUpdatedMessageAsync, cancellationToken);
        }

   
        private async Task ProcessProductCreatedMessageAsync(object? sender, BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation requested, skipping message processing.");
                return;
            }

            // Create a scope to resolve the DbContext
            using var scope = _serviceScopeFactory.CreateScope();

            // Resolve the DbContext from the scope
            var orderDbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            // Deserialize the message body to the ProductCreatedEventDto
            string message = Encoding.UTF8.GetString(args.Body.ToArray());

            ProductCreatedEventDto productCreatedEventDto = JsonSerializer.Deserialize<ProductCreatedEventDto>(message);

            if (productCreatedEventDto == null)
            {
                _logger.LogError("Product Created Event is null");
                return;
            }


            // Check if the product already exists in the database
            var dbProduct = orderDbContext.Products.SingleOrDefault(x => x.ProductId == productCreatedEventDto.ProductId);

            // If the product does not exist, add it to the database
            if (dbProduct == null)
            {
                await orderDbContext.Products.AddAsync(new Product
                {
                    Name = productCreatedEventDto.Name,
                    ProductId = productCreatedEventDto.ProductId,
                    Quantity = productCreatedEventDto.Quantity
                }, cancellationToken);
            }
            else
            {
                _logger.LogError("Product is already created");
                return;
            }


            // Save the changes to the database
            await orderDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Product Created Message Recieved: {message}");


        }


        private async Task ProcessProductUpdatedMessageAsync(object? sender, BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation requested, skipping message processing.");
                return;
            }

            // Create a scope to resolve the DbContext
            using var scope = _serviceScopeFactory.CreateScope();

            // Resolve the DbContext from the scope
            var orderDbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            // Deserialize the message body to the ProductCreatedEventDto
            string message = Encoding.UTF8.GetString(args.Body.ToArray());

            ProductUpdatedEventDto producUpdatedEventDto = JsonSerializer.Deserialize<ProductUpdatedEventDto>(message);

            if (producUpdatedEventDto == null)
            {
                _logger.LogError("Product Updated Event is null");
                return;
            }


            // Check if the product already exists in the database
            var dbProduct = orderDbContext.Products.SingleOrDefault(x => x.ProductId == producUpdatedEventDto.ProductId);

            // If the product does not exist, add it to the database
            if (dbProduct == null)
            {
                _logger.LogError("Product is not existed");
            }
            else
            {
                // If the product exists, update its properties
                dbProduct.Quantity = producUpdatedEventDto.Quantity;
            }


            // Save the changes to the database
            await orderDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Product Updated Message Recieved: {message}");


        }
    }
}
