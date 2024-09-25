using Foundation.Abstractions.Models;

namespace ShoppingCart.API.Models
{
    public class Cart : Entity<Guid>
    {
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public List<ProductSelection> Selections { get; set; } = default!;
        public decimal TotalPrice => Selections?.Sum(x => x.Price * x.Quantity) ?? 0;
    }
}
