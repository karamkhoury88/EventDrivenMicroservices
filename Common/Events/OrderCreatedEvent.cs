using Common.Events.Dtos;
using Common.Services;

namespace Common.Events
{
    public class OrderCreatedEvent(OrderCreatedEventDto? data, IMessageQueueService svc) : Event<OrderCreatedEventDto>(data, svc)
    {
        protected override string EventName { get; } = "order.created";
        protected override string ExchangeName { get; } = Exchanges.OrderExchange;
    }
}
