using Catalog.Domain.Abstractions;

namespace Catalog.Domain.Models
{
    public class Product : Entity<Guid>
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public Guid? CategoryId { get; set; }
        public string Description { get; set; } = default!;
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
