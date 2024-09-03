using Catalog.Domain.Abstractions;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Models
{
    public class Product : Entity<ProductId>
    {
        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
        public CategoryId CategoryId { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public Category Category { get; set; } = default!;

        public static Product Create(string name, 
            decimal price, 
            CategoryId categoryId, 
            string? description = null)
        {
            var product = new Product
            {
                Id = ProductId.Of(Guid.NewGuid()),
                Name = name,
                Price = price,
                Description = description ?? string.Empty,
                CategoryId = categoryId
            };

            return product;
        }
    }
}
