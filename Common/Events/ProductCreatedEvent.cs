using Common.Events.Dtos;
using Common.Services;

namespace Common.Events
{
    public class ProductCreatedEvent(ProductCreatedEventDto? data, IMessageQueueService svc) : Event<ProductCreatedEventDto>(data, svc)
    {
        protected override string EventName { get; } = "product.created";
        protected override string ExchangeName { get; } = Exchanges.InventoryExchange;
    }
}
