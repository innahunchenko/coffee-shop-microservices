using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Services.Categories;
using Catalog.Application.Mapping;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            this.categoryRepository = categoryRepository;
            this.logger = logger;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken ct)
        {
            var categoriesFromDb = await categoryRepository.GetMainCategoriesWithSubcategoriesAsync(ct);
            
            if (categoriesFromDb.Any())
            {
                logger.LogInformation($"{categoriesFromDb.Count} categories retrieved from db");
                return categoriesFromDb.Select(category => category.ToCategoryDto()).ToList();
            }

            return new List<CategoryDto>();
        }
    }
}

