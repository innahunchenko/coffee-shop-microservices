using Carter;
using Catalog.Application.Products.Queries;
using MediatR;

namespace Catalog.API.Endpoints
{
    public class ProductsEndpoint : CarterModule
    {
        public ProductsEndpoint() 
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

            app.MapGet("/{id}", async (Guid id, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdRequest(id), ct);
                return Results.Ok(result);
            });

            app.MapPost("/ids", async (List<Guid> ids, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(new GetProductsByIdsRequest(ids), ct);
                return Results.Ok(result);
            });
        }
    }
}
