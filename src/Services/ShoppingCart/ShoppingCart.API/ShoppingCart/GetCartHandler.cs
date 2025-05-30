﻿using MediatR;
using ShoppingCart.API.Models;
using ShoppingCart.API.Services;

namespace ShoppingCart.API.ShoppingCart
{
    public record GetCartRequest(string? userId) : IRequest<Cart>;

    public sealed class GetCartHandler(IShoppingCartService service) : IRequestHandler<GetCartRequest, Cart>
    {
        public async Task<Cart> Handle(GetCartRequest request, CancellationToken cancellationToken)
        {
            return await service.GetOrCreateCartAsync(cancellationToken);
        }
    }
}
