using Catalog.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Data
{
    public static class DatabaseExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();

            await SeedCategoriesAsync(context);
            await SeedProductsAsync(context);
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            var coffeeCategory = Category.Create("Coffee");
            var accessoriesCategory = Category.Create("Accessories");
            var foodItemsCategory = Category.Create("FoodItems");
            var othersCategory = Category.Create("Others");

            var categories = new List<Category>
            {
                coffeeCategory,
                accessoriesCategory,
                foodItemsCategory,
                othersCategory,

                Category.Create("WholeBean", coffeeCategory.Id),
                Category.Create("Ground", coffeeCategory.Id),
                Category.Create("Instant", coffeeCategory.Id),
                Category.Create("Pods", coffeeCategory.Id),
                Category.Create("Specialty", coffeeCategory.Id),

                Category.Create("CoffeeMakers", accessoriesCategory.Id),
                Category.Create("Grinders", accessoriesCategory.Id),
                Category.Create("Mugs", accessoriesCategory.Id),
                Category.Create("Filters", accessoriesCategory.Id),

                Category.Create("Pastries", foodItemsCategory.Id),
                Category.Create("Snacks", foodItemsCategory.Id),
                Category.Create("Syrups", foodItemsCategory.Id),
                Category.Create("MilkAlternatives", foodItemsCategory.Id),

                Category.Create("Merchandise", othersCategory.Id),
                Category.Create("Subscriptions", othersCategory.Id)
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            var random = new Random();
            var categories = await context.Categories.ToListAsync();
            var subcategories = categories.Where(c => c.ParentCategoryId != null).ToList();
            var parentCategories = categories.Where(c => c.ParentCategoryId == null).ToList();

            var products = new List<Product>();

            foreach (var subcategory in subcategories)
            {
                var parentCategory = parentCategories.First(pc => pc.Id == subcategory.ParentCategoryId);
                int productCount = random.Next(5, 11);

                for (int j = 0; j < productCount; j++)
                {
                    var productName = $"{parentCategory.Name} {subcategory.Name} Product {j + 1}";
                    var productDescription = $"Description for {parentCategory.Name} {subcategory.Name} Product {j + 1}";

                    products.Add(Product.Create(
                        productName,
                        (decimal)(random.NextDouble() * 100),
                        subcategory.Id,
                        "logo.png",
                        productDescription
                    ));
                }
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
