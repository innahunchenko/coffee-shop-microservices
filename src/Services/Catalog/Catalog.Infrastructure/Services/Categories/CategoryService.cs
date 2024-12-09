using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Services.Categories;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categoriesFromDb = await categoryRepository.GetAllCategoriesAsync();
            
            if (categoriesFromDb.Any())
            {
                return categoriesFromDb;
            }

            return new List<CategoryDto>();
        }

        public async Task<Guid> AddCategoryAsync(string name, string? parentCategoryName)
        {
            var id = await categoryRepository.AddCategoryAsync(name, parentCategoryName);
            return id;
        }

        public async Task UpdateCategoryAsync(string oldName, string newName)
        {
            await categoryRepository.UpdateCategoryAsync(oldName, newName);
        }

        public async Task<CategoryDto> GetCategoryByNameAsync(string name)
        {
            var category = await categoryRepository.GetCategoryByName(name);
            return category;
        }

        public async Task DeleteCategoryAsync(string categoryName)
        {
            await categoryRepository.DeleteCategoryAsync(categoryName);
        }
    }
}

