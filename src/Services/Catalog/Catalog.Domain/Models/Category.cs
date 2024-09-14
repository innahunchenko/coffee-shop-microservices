using Catalog.Domain.Abstractions;

namespace Catalog.Domain.Models
{
    public class Category : Entity<Guid>
    {
        public string Name { get; set; } = default!;
        public Category? ParentCategory { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public ICollection<Category> Subcategories { get; set; } = new List<Category>();

        public static Category Create(string name, Guid? parentCategoryId = null)
        {
            return new Category()
            {
                Id = Guid.NewGuid(),
                Name = name,
                ParentCategoryId = parentCategoryId,
                ParentCategory = null,
                Subcategories = new List<Category>()
            };
        }
    }
}
