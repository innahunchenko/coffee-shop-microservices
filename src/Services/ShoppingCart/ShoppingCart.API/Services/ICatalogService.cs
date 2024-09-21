using Catalog.Domain.Models.Dtos;
using Refit;

namespace ShoppingCart.API.Services
{
    public interface ICatalogService
    {
        [Get("/products/")]
        Task<ProductDto> GetProductByIdAsync(Guid productId);
    }
}
