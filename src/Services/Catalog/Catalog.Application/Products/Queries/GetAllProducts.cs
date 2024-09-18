using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Services.Products;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetAllRequest(int PageNumber, int PageSize) : PaginationParameters(PageNumber, PageSize), IRequest<PaginatedList<ProductDto>>;

    public sealed class GetAllHandler : IRequestHandler<GetAllRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetAllHandler(IProductService productService) 
        { 
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return productService.GetAllAsync(
                new PaginationParameters(request.PageNumber, request.PageSize));
        }
    }
}
