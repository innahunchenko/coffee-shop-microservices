using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Services.Products;
using MediatR;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Application.Products.Queries
{
    public record GetByNameRequest(string Name, int PageNumber, int PageSize) : PaginationParameters(PageNumber, PageSize), IRequest<PaginatedList<ProductDto>>;

    public sealed class GetByNameHandler : IRequestHandler<GetByNameRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetByNameHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetByNameRequest request, CancellationToken cancellationToken)
        {
            return productService.GetByNameAsync(
                request.Name, 
                new PaginationParameters(request.PageNumber, request.PageSize));
        }
    }
}
