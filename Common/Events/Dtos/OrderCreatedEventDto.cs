namespace Common.Events.Dtos
{
    public record OrderCreatedEventDto : EventDto
    {
        public Guid ProductId { get; init; }
        public int NumberOfItems { get; init; }

    }
}
