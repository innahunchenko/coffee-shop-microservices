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
            await categoryCacheService.AddOrUpdateCategoryInCacheAsync(name, parentCategoryName);
            return categoryId;
        }

        public async Task UpdateCategoryAsync(string oldName, string newName, string? parentCategoryName)
        {
            await categoryService.UpdateCategoryAsync(oldName, newName, parentCategoryName);
            await categoryCacheService.RemoveCategoryFromCacheAsync(oldName);
            await categoryCacheService.AddOrUpdateCategoryInCacheAsync(newName, parentCategoryName);
        }

        public async Task DeleteCategoryAsync(string categoryName)
        {
            await categoryService.DeleteCategoryAsync(categoryName);
            await categoryCacheService.RemoveCategoryFromCacheAsync(categoryName);
        }
    }
}
