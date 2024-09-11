using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Services.Categories;
using Catalog.Application.Mapping;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var categoriesFromDb = await categoryRepository.GetMainCategoriesWithSubcategoriesAsync(cancellationToken);
            
            if (categoriesFromDb.Any())
            {
                Console.WriteLine($"Categories from db");
                return categoriesFromDb.Select(category => category.ToCategoryDto()).ToList();
            }

            return new List<CategoryDto>();
        }
    }
}

