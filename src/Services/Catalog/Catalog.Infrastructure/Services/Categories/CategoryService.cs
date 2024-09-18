using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Services.Categories;
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

        public async Task<List<CategoryDto>> GetAsync()
        {
            var categoriesFromDb = await categoryRepository.GetAllAsync();
            
            if (categoriesFromDb.Any())
            {
                logger.LogInformation($"{categoriesFromDb.Count} categories retrieved from db");
                return categoriesFromDb;
            }

            return new List<CategoryDto>();
        }
    }
}

