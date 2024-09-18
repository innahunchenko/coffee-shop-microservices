using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Products;
using Catalog.Domain.Services.Products;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductService> logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            this.productRepository = productRepository;
            this.logger = logger;
        }

        public async Task<PaginatedList<ProductDto>> GetBySubcategoryAsync(string subcategory, 
            PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetBySubcategoryAsync(subcategory, paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetTotalCountBySubcategoryAsync(subcategory);
            logger.LogInformation($"{products.Count()} products retrieved from db by subcategory");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetByCategoryAsync(string category, 
            PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetByCategoryAsync(category, paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetTotalCountByCategoryAsync(category);
            logger.LogInformation($"{products.Count()} products retrieved from db by category");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetByNameAsync(string name, 
            PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetByProductNameAsync(name, paginationParameters);
            if (products == null || products.Count() == 0)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetTotalCountByNameAsync(name);
            logger.LogInformation($"{products.Count()} products retrieved from db by name");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetAllAsync(PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetAllAsync(paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetAllTotalCountAsync();
            logger.LogInformation($"{products.Count()} products retrieved from db");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }
    }
}
