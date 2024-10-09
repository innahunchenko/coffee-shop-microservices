using Catalog.Domain.Models.Dtos;
using Refit;

namespace ShoppingCart.API.Services
{
    public interface ICatalogService
    {
        [Post("/catalog/products/ids")]
        Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(IEnumerable<Guid> ids);
    }
}
