using Carter;
using Catalog.Application.Products.Queries;
using MediatR;

namespace Catalog.API.Endpoints
{
    public class GetProductsEndpoint : CarterModule
    {
        public GetProductsEndpoint() 
            : base("/products") { }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", async ([AsParameters] GetAllRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            }).CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromMinutes(10)));

            app.MapGet("/category", async ([AsParameters] GetByCategoryRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            }).CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromMinutes(10)));

            app.MapGet("/subcategory", async ([AsParameters] GetBySubcategoryRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            }).CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromMinutes(10)));

            app.MapGet("/name", async ([AsParameters] GetByNameRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            }).CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromMinutes(10)));
        }
    }
}
