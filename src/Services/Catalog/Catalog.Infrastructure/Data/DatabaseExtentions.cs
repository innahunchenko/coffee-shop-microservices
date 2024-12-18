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

            var categories = new List<Category>
            {
                coffeeCategory,
                accessoriesCategory,
                foodItemsCategory,

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
                Category.Create("MilkAlternatives", foodItemsCategory.Id)
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

            var products = new List<Product>();

            foreach (var subcategory in subcategories)
            {
                switch (subcategory.Name)
                {
                    // Coffee subcategories
                    case "WholeBean":
                        products.Add(Product.Create("Ethiopian Yirgacheffe Whole Bean", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_1.jpg", "Aromatic coffee with floral and citrus notes, sourced from Ethiopia."));
                        products.Add(Product.Create("Colombian Supremo Whole Bean", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_2.jpg", "Rich and smooth whole bean coffee with a nutty finish."));
                        products.Add(Product.Create("Guatemalan Antigua Whole Bean", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_3.jpg", "Full-bodied coffee with notes of chocolate and caramel."));
                        products.Add(Product.Create("Kenyan AA Whole Bean", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_4.jpg", "Bright and vibrant coffee with a citrusy finish."));
                        break;

                    case "Ground":
                        products.Add(Product.Create("Italian Espresso Ground Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_1.jpg", "Perfectly ground coffee for a strong and bold espresso."));
                        products.Add(Product.Create("French Roast Ground Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_2.jpg", "Dark roasted ground coffee with a smoky aroma."));
                        products.Add(Product.Create("Breakfast Blend Ground Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_3.jpg", "Mild and balanced coffee, ideal for your morning routine."));
                        products.Add(Product.Create("Hazelnut Flavored Ground Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_4.jpg", "Ground coffee with a delightful hazelnut flavor."));
                        break;

                    case "Instant":
                        products.Add(Product.Create("Classic Colombian Instant Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_1.jpg", "Quick and easy Colombian coffee with a full-bodied taste."));
                        products.Add(Product.Create("Vanilla Flavored Instant Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_2.jpg", "Instant coffee with a hint of creamy vanilla flavor."));
                        products.Add(Product.Create("Mocha Instant Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_3.jpg", "Rich and chocolatey instant coffee for indulgent moments."));
                        products.Add(Product.Create("Caramel Flavored Instant Coffee", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_4.jpg", "Sweet caramel-flavored coffee for a delightful treat."));
                        break;

                    case "Pods":
                        products.Add(Product.Create("Nespresso Compatible Espresso Pods", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_1.jpg", "Convenient pods for a rich and bold espresso experience."));
                        products.Add(Product.Create("Decaf Coffee Pods", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_2.jpg", "Decaffeinated coffee pods with a smooth and balanced flavor."));
                        products.Add(Product.Create("Hazelnut Flavored Coffee Pods", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_3.jpg", "Coffee pods with a hint of nutty hazelnut flavor."));
                        products.Add(Product.Create("French Vanilla Coffee Pods", (decimal)(random.NextDouble() * 100), subcategory.Id, "coffee_4.jpg", "Smooth and creamy vanilla-flavored coffee pods."));
                        break;

                    // Accessories subcategories
                    case "CoffeeMakers":
                        products.Add(Product.Create("Automatic Espresso Machine", (decimal)(random.NextDouble() * 100), subcategory.Id, "makers_1.jpg", "An advanced espresso machine with programmable settings."));
                        products.Add(Product.Create("French Press Coffee Maker", (decimal)(random.NextDouble() * 100), subcategory.Id, "makers_2.jpg", "A classic French press for rich and aromatic coffee."));
                        products.Add(Product.Create("Pour-Over Coffee Maker", (decimal)(random.NextDouble() * 100), subcategory.Id, "makers_1.jpg", "A pour-over coffee maker for precision brewing."));
                        products.Add(Product.Create("Single-Serve Coffee Maker", (decimal)(random.NextDouble() * 100), subcategory.Id, "makers_2.jpg", "Compact coffee maker perfect for single servings."));
                        break;

                    case "Grinders":
                        products.Add(Product.Create("Burr Coffee Grinder", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Premium grinder with adjustable settings for consistent grind."));
                        products.Add(Product.Create("Blade Coffee Grinder", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Fast and efficient coffee grinder with sharp blades."));
                        products.Add(Product.Create("Manual Coffee Grinder", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "A portable manual grinder for on-the-go brewing."));
                        products.Add(Product.Create("Electric Coffee Grinder", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Electric grinder for quick and precise coffee preparation."));
                        break;

                    case "Mugs":
                        products.Add(Product.Create("Insulated Travel Mug", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Keeps your coffee hot for hours, ideal for travel."));
                        products.Add(Product.Create("Ceramic Coffee Mug", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Classic ceramic mug with a sleek design."));
                        products.Add(Product.Create("Glass Coffee Mug", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Transparent mug for enjoying your coffee visually."));
                        products.Add(Product.Create("Reusable Coffee Cup", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Eco-friendly reusable cup for sustainable coffee consumption."));
                        break;

                    case "Filters":
                        products.Add(Product.Create("Reusable Coffee Filter", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Durable and eco-friendly coffee filter for repeated use."));
                        products.Add(Product.Create("Paper Coffee Filters", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Classic disposable filters for a clean coffee taste."));
                        products.Add(Product.Create("Metal Mesh Filter", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Fine mesh filter for precision brewing."));
                        products.Add(Product.Create("Cold Brew Coffee Filter", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Special filter for smooth and refreshing cold brew coffee."));
                        break;

                    // FoodItems subcategories
                    case "Pastries":
                        products.Add(Product.Create("Croissant", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Freshly baked buttery croissant."));
                        products.Add(Product.Create("Chocolate Muffin", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Rich chocolate muffin with a soft texture."));
                        products.Add(Product.Create("Blueberry Scone", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Flaky scone filled with juicy blueberries."));
                        products.Add(Product.Create("Almond Danish", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Sweet pastry topped with sliced almonds."));
                        break;

                    case "Snacks":
                        products.Add(Product.Create("Granola Bar", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Healthy snack made with oats, nuts, and honey."));
                        products.Add(Product.Create("Trail Mix", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Mix of nuts, dried fruits, and chocolate chips."));
                        products.Add(Product.Create("Cheese Crackers", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Savory crackers with a hint of cheese."));
                        products.Add(Product.Create("Energy Bites", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Small bites packed with energy and flavor."));
                        break;

                    case "Syrups":
                        products.Add(Product.Create("Vanilla Syrup", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Sweet and creamy vanilla syrup for coffee."));
                        products.Add(Product.Create("Caramel Syrup", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Rich caramel syrup for a buttery sweetness."));
                        products.Add(Product.Create("Hazelnut Syrup", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Nutty and smooth hazelnut syrup."));
                        products.Add(Product.Create("Pumpkin Spice Syrup", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Seasonal syrup with warm spices."));
                        break;

                    case "MilkAlternatives":
                        products.Add(Product.Create("Almond Milk", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Dairy-free milk alternative made from almonds."));
                        products.Add(Product.Create("Soy Milk", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Plant-based milk with a creamy texture."));
                        products.Add(Product.Create("Oat Milk", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Smooth and slightly sweet milk alternative."));
                        products.Add(Product.Create("Coconut Milk", (decimal)(random.NextDouble() * 100), subcategory.Id, "logo.png", "Rich and tropical milk alternative from coconuts."));
                        break;
                }
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
