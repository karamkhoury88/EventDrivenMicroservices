using OrderService.Data;

namespace OrderService.Controllers.Dtos
{
    public record OrderDto
    {
        public string CustomerName { get; set; }
        public Guid ProductId { get; set; }
        public int NumberOfItems { get; set; }

        public OrderDto()
        {
            
        }
        public OrderDto(Order order)
        {
            ProductId = order.ProductId;
            CustomerName = order.CustomerName;
            NumberOfItems = order.NumberOfItems;
        }
    }
}
