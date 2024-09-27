using Foundation.Abstractions.Models;

namespace ShoppingCart.API.Models
{
    public class Cart : Entity<Guid>
    {
        public string? UserId { get; set; }
        public string? CartId { get; set; }
        public List<ProductSelection> Selections { get; set; } = new List<ProductSelection>();
        public decimal TotalPrice => Selections?.Sum(x => x.Price * x.Quantity) ?? 0;
    }
}
