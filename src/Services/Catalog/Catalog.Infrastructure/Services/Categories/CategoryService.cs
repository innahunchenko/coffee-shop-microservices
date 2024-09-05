using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Services.Categories;
using MapsterMapper;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var categoriesFromDb = await categoryRepository.GetMainCategoriesWithSubcategoriesAsync(cancellationToken);
            
            if (categoriesFromDb.Any())
            {
                Console.WriteLine($"Categories from db");
                return mapper.Map<List<CategoryDto>>(categoriesFromDb);
            }

            return new List<CategoryDto>();
        }
    }
}

