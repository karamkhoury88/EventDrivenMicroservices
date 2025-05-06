using Common.Events.Dtos;
using Common.Services;

namespace Common.Events
{
    public class ProductUpdatedEvent(ProductUpdatedEventDto? data, IMessageQueueService svc) : Event<ProductUpdatedEventDto>(data, svc)
    {
        protected override string EventName { get; } = "product.updated";
        protected override string ExchangeName { get; } = Exchanges.InventoryExchange;
       
    }
}
