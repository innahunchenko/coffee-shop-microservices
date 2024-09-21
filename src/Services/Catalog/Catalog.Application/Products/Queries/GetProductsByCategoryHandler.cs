using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Services.Products;
using MediatR;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsByCategoryRequest(string Category, int PageNumber, int PageSize) 
        : PaginationParameters(PageNumber, PageSize), IRequest<PaginatedList<ProductDto>>;

    public sealed class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryRequest, PaginatedList<ProductDto>>
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
                new PaginationParameters(request.PageNumber, request.PageSize));
        }
    }
}
