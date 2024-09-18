using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Services.Products;
using MediatR;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Application.Products.Queries
{
    public record GetByCategoryRequest(string Category, int PageNumber, int PageSize) 
        : PaginationParameters(PageNumber, PageSize), IRequest<PaginatedList<ProductDto>>;

    public sealed class GetByCategoryHandler : IRequestHandler<GetByCategoryRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetByCategoryHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetByCategoryRequest request, CancellationToken cancellationToken)
        {
            return productService.GetByCategoryAsync(
                request.Category, 
                new PaginationParameters(request.PageNumber, request.PageSize));
        }
    }
}
