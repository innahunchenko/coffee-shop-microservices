using Catalog.Domain.Services.Categories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Categories.Commands
{
    public record CreateCategoryRequest(string Name, string ParentCategoryName) : IRequest<IResult>;

    public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryRequest, IResult>
    {
        private readonly ICategoryService categoryService;
        public CreateCategoryHandler(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IResult> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var id = await categoryService.AddCategoryAsync(request.Name, request.ParentCategoryName);
            
            if (id == Guid.Empty)
            {
                return Results.Problem($"Category {request.Name} was not added");
            }

            return Results.Ok();
        }
    }
}
