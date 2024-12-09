using Catalog.Domain.Services.Categories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Categories.Commands
{
    public record DeleteCategoryRequest(string Name) : IRequest<IResult>;

    public sealed class DeleteCategoryHandler : IRequestHandler<DeleteCategoryRequest, IResult>
    {
        private readonly ICategoryService categoryService;
        public DeleteCategoryHandler(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IResult> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            await categoryService.DeleteCategoryAsync(request.Name);
            return Results.Ok();
        }
    }
}
