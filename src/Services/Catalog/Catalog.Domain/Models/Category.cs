using Catalog.Domain.Abstractions;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Models
{
    public class Category : Entity<CategoryId>
    {
        public string Name { get; private set; } = default!;
        public Category? ParentCategory { get; private set; }
        public CategoryId ParentCategoryId { get; private set; } = CategoryId.Empty();
        public ICollection<Category> Subcategories { get; private set; } = new List<Category>();

        public static Category Create(string name, CategoryId? parentCategoryId = null)
        {
            return new Category()
            {
                Id = CategoryId.New(),
                Name = name,
                ParentCategoryId = parentCategoryId ?? CategoryId.Empty(),
                ParentCategory = null,
                Subcategories = new List<Category>()
            };
        }
    }
}
