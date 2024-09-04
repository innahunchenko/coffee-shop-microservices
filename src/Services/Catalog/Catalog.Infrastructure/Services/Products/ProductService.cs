using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Products;
using Catalog.Domain.Services.Products;
using MapsterMapper;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;

        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
        }

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetProductsBySubcategoryAsync(subcategory, paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var mappedProducts = mapper.Map<IEnumerable<ProductDto>>(products);
            var totalProducts = await productRepository.GetSubcategoryProductsTotalCountAsync(subcategory);
            Console.WriteLine($"Products {totalProducts} from db");
            return new PaginatedList<ProductDto>(mappedProducts, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetProductsByCategoryAsync(category, paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var mappedProducts = mapper.Map<IEnumerable<ProductDto>>(products);
            var totalProducts = await productRepository.GetCategoryProductsTotalCountAsync(category);
            Console.WriteLine($"Products {totalProducts} from db");
            return new PaginatedList<ProductDto>(mappedProducts, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string name, PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetProductsByProductNameAsync(name, paginationParameters);
            if (products == null || products.Count() == 0)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var mappedProducts = mapper.Map<IEnumerable<ProductDto>>(products);
            var totalProducts = await productRepository.GetProductNameTotalCountAsync(name);
            Console.WriteLine($"Products {totalProducts} from db");
            return new PaginatedList<ProductDto>(mappedProducts, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetAllProductsAsync(paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var mappedProducts = mapper.Map<IEnumerable<ProductDto>>(products);
            var totalProducts = await productRepository.GetAllProductsTotalCountAsync();
            Console.WriteLine($"Products {totalProducts} from db");
            return new PaginatedList<ProductDto>(mappedProducts, totalProducts, paginationParameters.PageSize);
        }
    }
}
