using Carter;
using Catalog.Application.Categories.Commands;
using Catalog.Application.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Endpoints
{
    public class CategoriesEndpoint : CarterModule
    {
        public CategoriesEndpoint()
            : base("/categories") { }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", async ([AsParameters] GetAllCategoriesRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });

            app.MapPost("/create", async ([FromBody] CreateCategoryRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });

            app.MapPost("/update", async ([FromBody] UpdateCategoryRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });

            app.MapPost("/delete", async ([FromBody] DeleteCategoryRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            });
        }        
    }
}
