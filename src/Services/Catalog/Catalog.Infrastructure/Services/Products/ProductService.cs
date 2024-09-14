using Catalog.Application.Mapping;
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

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, 
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetProductsBySubcategoryAsync(subcategory, paginationParameters, cancellationToken);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var productsDto = products.Select(product => product.ToProductDto());
            var totalProducts = await productRepository.GetSubcategoryProductsTotalCountAsync(subcategory);
            logger.LogInformation($"{products.Count()} products retrieved from db by subcategory");
            return new PaginatedList<ProductDto>(productsDto, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, 
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetProductsByCategoryAsync(category, paginationParameters, cancellationToken);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var productsDto = products.Select(product => product.ToProductDto());
            var totalProducts = await productRepository.GetCategoryProductsTotalCountAsync(category);
            logger.LogInformation($"{products.Count()} products retrieved from db by category");
            return new PaginatedList<ProductDto>(productsDto, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string name, 
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetProductsByProductNameAsync(name, paginationParameters, cancellationToken);
            if (products == null || products.Count() == 0)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var productsDto = products.Select(product => product.ToProductDto());
            var totalProducts = await productRepository.GetProductNameTotalCountAsync(name);
            logger.LogInformation($"{products.Count()} products retrieved from db by name");
            return new PaginatedList<ProductDto>(productsDto, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetAllProductsAsync(paginationParameters, cancellationToken);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var productsDto = products.Select(product => product.ToProductDto());
            var totalProducts = await productRepository.GetAllProductsTotalCountAsync();
            logger.LogInformation($"{products.Count()} products retrieved from db");
            return new PaginatedList<ProductDto>(productsDto, totalProducts, paginationParameters.PageSize);
        }
    }
}
