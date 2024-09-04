using Catalog.Domain.Abstractions;

namespace Catalog.Domain.Models
{
    public class Product : Entity<Guid>
    {
        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
        public Guid? CategoryId { get; private set; }
        public string Description { get; private set; } = default!;
        public Category? Category { get; set; }

        public static Product Create(string name, 
            decimal price, 
            Guid categoryId, 
            string? description = null)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Price = price,
                Description = description ?? string.Empty,
                CategoryId = categoryId
            };

            return product;
        }
    }
}
