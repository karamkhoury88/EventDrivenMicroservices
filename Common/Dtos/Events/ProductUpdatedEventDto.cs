namespace Common.Dtos.Events
{
    public record ProductUpdatedEventDto : EventDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }

        public static string EventName = "product.updated";
    }
}
