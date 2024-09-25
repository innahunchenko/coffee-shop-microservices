using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.API.Models;
using ShoppingCart.API.ShoppingCart.Store;
using System.Collections.Generic;

namespace ShoppingCart.API.ShoppingCart.Get
{
    public class GetCartEndpoints : CarterModule
    {
        public GetCartEndpoints()
            : base("/shopping-cart") { }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", async ([FromQuery] string? userId, CancellationToken ct, ISender sender) =>
            {
                //var cacheKey = $"cart_{cartId}";
                //var cachedCart = await cacheStore.GetAsync(cacheKey, ct);

                //if (cachedCart != null)
                //{
                //    return Results.Ok(cachedCart);
                //}

                //var request = new GetCartRequest(cartId);
                //var result = await sender.Send(request, ct);

                //var resultBytes = JsonSerializer.SerializeToUtf8Bytes(result);

                //var expiration = TimeSpan.FromMinutes(10);

                //await cacheStore.SetAsync(cacheKey, resultBytes, null, expiration, ct);
                var result = await sender.Send(new GetCartRequest(userId), ct);
                return Results.Ok(result);
            });

            app.MapPost("/", async(List<ProductSelection> selections, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(new StoreCartRequest(selections), ct);
                return Results.Ok(result);
            });
        }
    }
}
