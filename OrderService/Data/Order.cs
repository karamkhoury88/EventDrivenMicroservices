namespace OrderService.Data
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public Guid ProductId { get; set; }
        public int NumberOfItems { get; set; }
    }
}
