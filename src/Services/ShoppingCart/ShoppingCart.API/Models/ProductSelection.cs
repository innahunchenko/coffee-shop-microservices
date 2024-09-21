namespace ShoppingCart.API.Models
{
    public class ProductSelection
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; } 
        public string ProductName { get; set; } = default!;
    }
}
