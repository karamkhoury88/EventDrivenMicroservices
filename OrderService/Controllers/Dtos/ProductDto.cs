using OrderService.Data;

namespace OrderService.Controllers.Dtos
{
    public record ProductDto
    {
        public string Name { get; init; }
        public int Quantity { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; }

        public ProductDto()
        {
            
        }
        public ProductDto(Product product)
        {
            Name = product.Name;
            Quantity = product.Quantity;
            ProductId = product.ProductId;
            ProductName = product.Name;
        }
    }
}
