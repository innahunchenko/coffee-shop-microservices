using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Services.Products;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsByIdsRequest(IEnumerable<Guid> productIds) : IRequest<IEnumerable<ProductDto>>;

    public sealed class GetProductsByIdsHandler : IRequestHandler<GetProductsByIdsRequest, IEnumerable<ProductDto>>
    {
        private readonly IProductService productService;
        public GetProductsByIdsHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsByIdsRequest request, CancellationToken cancellationToken)
        {
            return await productService.GetProductsByIdsAsync(request.productIds);
        }
    }
}
