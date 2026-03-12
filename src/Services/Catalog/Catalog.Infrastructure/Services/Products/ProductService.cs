using Catalog.Application.Exceptions;
using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Products;
using Catalog.Domain.Services.Products;
using Foundation.Exceptions;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(
            string subcategory,
            PaginationParameters paginationParameters)
        {
            var products = await productRepository
                .GetProductsBySubcategoryAsync(subcategory, paginationParameters)
                ?? Enumerable.Empty<ProductDto>();

            var totalProducts = products.Any()
                ? await productRepository.GetProductsTotalCountBySubcategoryAsync(subcategory)
                : 0;

            return new PaginatedList<ProductDto>(
                products,
                totalProducts,
                paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(
            string category,
            PaginationParameters paginationParameters)
        {
            var products = await productRepository
                .GetProductsByCategoryAsync(category, paginationParameters)
                ?? Enumerable.Empty<ProductDto>();

            var totalProducts = products.Any()
                ? await productRepository.GetProductsTotalCountByCategoryAsync(category)
                : 0;

            return new PaginatedList<ProductDto>(
                products,
                totalProducts,
                paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByNameAsync(
            string name,
            PaginationParameters paginationParameters)
        {
            var products = await productRepository
                .GetProductsByNameAsync(name, paginationParameters)
                ?? Enumerable.Empty<ProductDto>();

            var totalProducts = products.Any()
                ? await productRepository.GetProductsTotalCountByNameAsync(name)
                : 0;

            return new PaginatedList<ProductDto>(
                products,
                totalProducts,
                paginationParameters.PageSize);
        }

        public async Task<PaginatedList<ProductDto>> GetAllProductsAsync(
            PaginationParameters paginationParameters)
        {
            var products = await productRepository
                .GetAllProductsAsync(paginationParameters)
                ?? Enumerable.Empty<ProductDto>();

            var totalProducts = products.Any()
                ? await productRepository.GetAllProductsTotalCountAsync()
                : 0;

            return new PaginatedList<ProductDto>(
                products,
                totalProducts,
                paginationParameters.PageSize);
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            return await productRepository.GetProductByIdAsync(productId)
                ?? throw new ProductNotFoundException(productId.ToString());
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(
            IEnumerable<Guid> productIds)
        {
            var products = await productRepository
                .GetProductsByIdsAsync(productIds)
                ?? Enumerable.Empty<ProductDto>();

            if (!products.Any())
            {
                throw new NotFoundException("Not found any products for provided ids");
            }

            return products;
        }
    }
}