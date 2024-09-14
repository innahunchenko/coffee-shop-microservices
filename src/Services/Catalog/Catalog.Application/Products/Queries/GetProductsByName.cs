using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Services.Products;
using MediatR;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsByNameRequest(string Name, int PageNumber, int PageSize) : PaginationParameters(PageNumber, PageSize), IRequest<PaginatedList<ProductDto>>;

    public sealed class GetProductsByNameHandler : IRequestHandler<GetProductsByNameRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetProductsByNameHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetProductsByNameRequest request, CancellationToken cancellationToken)
        {
            return productService.GetProductsByNameAsync(
                request.Name, 
                new PaginationParameters(request.PageNumber, request.PageSize), cancellationToken);
        }
    }
}
