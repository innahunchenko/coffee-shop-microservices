using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Services.Products;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetAllProductsRequest(int PageNumber, int PageSize) : PaginationParameters(PageNumber, PageSize), IRequest<PaginatedList<ProductDto>>;

    public sealed class GetAllProductsHandler : IRequestHandler<GetAllProductsRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetAllProductsHandler(IProductService productService) 
        { 
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            return productService.GetAllProductsAsync(
                new PaginationParameters(request.PageNumber, request.PageSize));
        }
    }
}
