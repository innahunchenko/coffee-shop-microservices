using Catalog.Application.Exceptions;
using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Products;
using Catalog.Domain.Services.Products;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetProductsBySubcategoryAsync(subcategory, paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetProductsTotalCountBySubcategoryAsync(subcategory);
            logger.LogInformation($"{products.Count()} products retrieved from db by subcategory");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetProductsByCategoryAsync(category, paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetProductsTotalCountByCategoryAsync(category);
            logger.LogInformation($"{products.Count()} products retrieved from db by category");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string name, PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetProductsByNameAsync(name, paginationParameters);
            if (products == null || products.Count() == 0)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetProductsTotalCountByNameAsync(name);
            logger.LogInformation($"{products.Count()} products retrieved from db by name");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetAllProductsAsync(paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetAllProductsTotalCountAsync();
            logger.LogInformation($"{products.Count()} products retrieved from db");
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            var product = await productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                logger.LogError($"Db Product {productId} has not found");
                throw new ProductNotFoundException(productId.ToString());
            }
            logger.LogInformation($"Db product {productId} is {JsonSerializer.Serialize(product)}");
            return product;
        }
    }
}
