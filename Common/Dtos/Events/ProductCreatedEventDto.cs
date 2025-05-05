namespace Common.Dtos.Events
{

    public record ProductCreatedEventDto : EventDto
    {
        public string Name { get; init; } = string.Empty;
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }

        public static string EventName = "product.created";
    }
}
