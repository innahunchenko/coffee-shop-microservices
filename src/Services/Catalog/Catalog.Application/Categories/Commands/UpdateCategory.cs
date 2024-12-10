using Catalog.Domain.Services.Categories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Categories.Commands
{
    public record UpdateCategoryRequest(string OldNameName, string NewName, string? ParentCategoryName) : IRequest<IResult>;

    public sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryRequest, IResult>
    {
        private readonly ICategoryService categoryService;
        public UpdateCategoryHandler(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IResult> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            await categoryService.UpdateCategoryAsync(request.OldNameName, request.NewName, request.ParentCategoryName);
            return Results.Ok();
        }
    }
}
