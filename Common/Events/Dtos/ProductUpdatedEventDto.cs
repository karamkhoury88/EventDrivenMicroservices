namespace Common.Events.Dtos
{
    public record ProductUpdatedEventDto : EventDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
