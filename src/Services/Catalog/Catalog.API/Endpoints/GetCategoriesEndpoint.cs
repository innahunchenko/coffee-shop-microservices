using Carter;
using Catalog.Application.Categories.Queries;
using MediatR;

namespace Catalog.API.Endpoints
{
    public class GetCategoriesEndpoint : CarterModule
    {
        public GetCategoriesEndpoint()
            : base("/categories") { }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", async ([AsParameters] GetAllRequest request, CancellationToken ct, ISender sender) =>
            {
                var result = await sender.Send(request, ct);
                return Results.Ok(result);
            }).CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromMinutes(10)));
        }        
    }
}
