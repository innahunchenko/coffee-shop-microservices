using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Services.Products;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetProductByIdRequest(Guid productId) : IRequest<ProductDto>;

    public sealed class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, ProductDto>
    {
        private readonly IProductService productService;
        public GetProductByIdHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<ProductDto> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            return productService.GetProductByIdAsync(request.productId);
        }
    }
}
