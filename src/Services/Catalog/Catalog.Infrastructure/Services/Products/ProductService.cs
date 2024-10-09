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

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters)
        {
            var products = await productRepository.GetProductsBySubcategoryAsync(subcategory, paginationParameters);
            if (products == null)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var totalProducts = await productRepository.GetProductsTotalCountBySubcategoryAsync(subcategory);
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
            return new PaginatedList<ProductDto>(products, totalProducts, paginationParameters.PageSize);
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            var product = await productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new ProductNotFoundException(productId.ToString());
            }

            return product;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds)
        {
            var products = await productRepository.GetProductsByIdsAsync(productIds);
            if (products == null)
            {
                throw new NotFoundException($"Not found any products from {products}");
            }

            return products;
        }
    }
}
