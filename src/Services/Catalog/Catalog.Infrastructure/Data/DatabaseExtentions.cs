using Catalog.Domain.Models;
using Catalog.Domain.ValueObjects;
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

            var coffeeCategoryId = CategoryId.New();
            var accessoriesCategoryId = CategoryId.New();
            var foodItemsCategoryId = CategoryId.New();
            var othersCategoryId = CategoryId.New();

            var categories = new List<Category>
            {
                Category.Create("Coffee"),
                Category.Create("Accessories"),
                Category.Create("FoodItems"),
                Category.Create("Others"),

                Category.Create("WholeBean", coffeeCategoryId),
                Category.Create("Ground", coffeeCategoryId),
                Category.Create("Instant", coffeeCategoryId),
                Category.Create("Pods", coffeeCategoryId),
                Category.Create("Specialty", coffeeCategoryId),

                Category.Create("CoffeeMakers", accessoriesCategoryId),
                Category.Create("Grinders", accessoriesCategoryId),
                Category.Create("Mugs", accessoriesCategoryId),
                Category.Create("Filters", accessoriesCategoryId),

                Category.Create("Pastries", foodItemsCategoryId),
                Category.Create("Snacks", foodItemsCategoryId),
                Category.Create("Syrups", foodItemsCategoryId),
                Category.Create("MilkAlternatives", foodItemsCategoryId),

                Category.Create("Merchandise", othersCategoryId),
                Category.Create("Subscriptions", othersCategoryId)
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
            var subcategoryIds = categories.Where(c => c.ParentCategoryId != null).Select(c => c.Id.Value).ToList();

            var products = new List<Product>();

            foreach (var subcategoryId in subcategoryIds)
            {
                int productCount = random.Next(5, 11);
                for (int j = 0; j < productCount; j++)
                {
                    products.Add(Product.Create(
                        $"{subcategoryId}_Product_{j + 1}",
                        (decimal)(random.NextDouble() * 100),
                        CategoryId.Of(subcategoryId),
                        $"Description for {subcategoryId}_Product_{j + 1}"
                    ));
                }
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
