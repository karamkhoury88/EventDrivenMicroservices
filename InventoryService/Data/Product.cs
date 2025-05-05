namespace InventoryService.Data
{
    public class Product
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public Product(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
            ProductId = Guid.NewGuid();
        }
    }
}
