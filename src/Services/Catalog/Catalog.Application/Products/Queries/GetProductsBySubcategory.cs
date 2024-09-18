using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Services.Products;
using MediatR;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Application.Products.Queries
{
    public record GetBySubcategoryRequest(string Subcategory, int PageNumber, int PageSize) : PaginationParameters(PageNumber, PageSize), IRequest<PaginatedList<ProductDto>>;

    public sealed class GetBySubcategoryHandler : IRequestHandler<GetBySubcategoryRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetBySubcategoryHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetBySubcategoryRequest request, CancellationToken cancellationToken)
        {
            return productService.GetBySubcategoryAsync(
                request.Subcategory, 
                new PaginationParameters(request.PageNumber, request.PageSize));
        }
    }
}
