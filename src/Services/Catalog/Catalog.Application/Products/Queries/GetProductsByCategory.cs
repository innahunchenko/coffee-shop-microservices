using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Services.Products;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsByCategoryRequest(string Category) : PaginationParameters(), IRequest<PaginatedList<ProductDto>>;

    internal sealed class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetProductsByCategoryHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetProductsByCategoryRequest request, CancellationToken cancellationToken)
        {
            return productService.GetProductsByCategoryAsync(
                request.Category, 
                new PaginationParameters() 
                { 
                    PageNumber = request.PageNumber, 
                    PageSize = request.PageSize 
                });
        }
    }
}
