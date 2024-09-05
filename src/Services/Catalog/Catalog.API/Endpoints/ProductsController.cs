using Catalog.Application.Products.Queries;
using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Foundation.Abstractions.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Endpoints
{
    public class ProductsController : ApiControllerBase
    {
        [HttpGet()]
        public Task<PaginatedList<ProductDto>> GetAllProducts([FromQuery] GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            return Mediator.Send(request, cancellationToken);
        }

        [HttpGet("category")]
        public Task<PaginatedList<ProductDto>> GetProductsByCategory([FromQuery] GetProductsByCategoryRequest request, CancellationToken cancellationToken)
        {
            return Mediator.Send(request, cancellationToken);
        }

        [HttpGet("name")]
        public Task<PaginatedList<ProductDto>> GetProductsByName([FromQuery] GetProductsByNameRequest request, CancellationToken cancellationToken)
        {
            return Mediator.Send(request, cancellationToken);
        }

        [HttpGet("subcategory")]
        public Task<PaginatedList<ProductDto>> GetProductsBySubcategory([FromQuery] GetProductsBySubcategoryRequest request, CancellationToken cancellationToken)
        {
            return Mediator.Send(request, cancellationToken);
        }
    }
}

