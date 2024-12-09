using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Services.Categories;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryServiceCacheDecorator : ICategoryService
    {
        private readonly ICategoryService categoryService;
        private readonly ICategoryCacheService categoryCacheService;

        public CategoryServiceCacheDecorator(
            ICategoryService categoryService,
            ICategoryCacheService categoryCacheService)
        {
            this.categoryService = categoryService;
            this.categoryCacheService = categoryCacheService;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var cachedCategories = await categoryCacheService.GetCategoriesFromCacheAsync();
            if (cachedCategories.Any())
            {
                return cachedCategories;
            }

            var categoriesFromDb = await categoryService.GetAllCategoriesAsync();
            if (categoriesFromDb.Any())
            {
                await categoryCacheService.ReloadCacheAsync(categoriesFromDb);
            }

            return categoriesFromDb;
        }

        public async Task<Guid> AddCategoryAsync(string name, string? parentCategoryName)
        {
            var categoryId = await categoryService.AddCategoryAsync(name, parentCategoryName);

            var newCategory = new CategoryDto
            {
                Name = name,
                Subcategories = new List<string>()
            };

            await categoryCacheService.AddOrUpdateCategoryInCacheAsync(newCategory);

            return categoryId;
        }

        public async Task UpdateCategoryAsync(string oldName, string newName)
        {
            await categoryService.UpdateCategoryAsync(oldName, newName);

            var existingCategory = await categoryCacheService.GetCategoryByNameFromCacheAsync(oldName);
            if (existingCategory != null)
            {
                await categoryCacheService.RemoveCategoryFromCacheAsync(existingCategory.Name);
            }

            var updatedCategory = new CategoryDto
            {
                Name = newName,
                Subcategories = existingCategory?.Subcategories ?? new List<string>()
            };

            await categoryCacheService.AddOrUpdateCategoryInCacheAsync(updatedCategory);
        }

        public async Task<CategoryDto> GetCategoryByNameAsync(string name)
        {
            var cachedCategory = await categoryCacheService.GetCategoryByNameFromCacheAsync(name);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            var categoryFromDb = await categoryService.GetCategoryByNameAsync(name);
            if (categoryFromDb != null)
            {
                await categoryCacheService.AddOrUpdateCategoryInCacheAsync(categoryFromDb);
            }

            return categoryFromDb;
        }

        public async Task DeleteCategoryAsync(string categoryName)
        {
            await categoryService.DeleteCategoryAsync(categoryName);

            var categories = await categoryCacheService.GetCategoriesFromCacheAsync();

            var categoryToDelete = categories.FirstOrDefault(c => c.Name == categoryName);
            if (categoryToDelete != null)
            {
                await categoryCacheService.RemoveCategoryFromCacheAsync(categoryToDelete.Name);
            }
        }
    }
}
