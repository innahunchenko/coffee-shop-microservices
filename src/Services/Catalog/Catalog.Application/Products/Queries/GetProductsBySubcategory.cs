using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Services.Products;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsBySubcategoryRequest(string Subcategory) : PaginationParameters(), IRequest<PaginatedList<ProductDto>>;

    internal sealed class GetProductsBySubcategoryHandler : IRequestHandler<GetProductsBySubcategoryRequest, PaginatedList<ProductDto>>
    {
        private readonly IProductService productService;
        public GetProductsBySubcategoryHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<PaginatedList<ProductDto>> Handle(GetProductsBySubcategoryRequest request, CancellationToken cancellationToken)
        {
            return productService.GetProductsBySubcategoryAsync(
                request.Subcategory, 
                new PaginationParameters() 
                { 
                    PageNumber = request.PageNumber, 
                    PageSize = request.PageSize 
                }, cancellationToken);
        }
    }
}
