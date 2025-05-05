namespace Common.Dtos.Events
{
    public record OrderCreatedEventDto : EventDto
    {
        public Guid ProductId { get; init; }
        public int NumberOfItems { get; init; }

        public static string EventName = "order.created";
    }
}
