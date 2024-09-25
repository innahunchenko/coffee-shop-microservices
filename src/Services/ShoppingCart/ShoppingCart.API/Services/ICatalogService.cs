using Catalog.Domain.Models.Dtos;
using Refit;

namespace ShoppingCart.API.Services
{
    public interface ICatalogService
    {
        [Get("/catalog/products/{id}")]
        Task<ProductDto> GetProductByIdAsync(Guid id);
    }
}
