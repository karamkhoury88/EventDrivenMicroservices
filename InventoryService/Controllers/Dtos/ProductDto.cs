using InventoryService.Data;

namespace InventoryService.Controllers.Dtos
{
    public record ProductDto
    {
        public string Name { get; init; }
        public int Quantity { get; init; }

        public ProductDto()
        {
            
        }
        public ProductDto(Product product)
        {
            Name = product.Name;
            Quantity = product.Quantity;
        }
    }
}
