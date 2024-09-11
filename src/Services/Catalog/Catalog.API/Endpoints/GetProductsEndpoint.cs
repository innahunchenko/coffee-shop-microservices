using Carter;
using Catalog.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Endpoints
{
    public class GetProductsEndpoint : CarterModule
    {
        public GetProductsEndpoint() 
            : base("/products") { }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", async ([AsParameters] GetAllProductsRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });

            app.MapGet("/category", async ([AsParameters] GetProductsByCategoryRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });

            app.MapGet("/subcategory", async ([AsParameters] GetProductsBySubcategoryRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });

            app.MapGet("/name", async ([AsParameters] GetProductsByNameRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });
        }
    }
}
