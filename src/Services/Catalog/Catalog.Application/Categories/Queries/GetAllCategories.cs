using Catalog.Domain.Models.Dtos;
using MediatR;
using Catalog.Domain.Services.Categories;

namespace Catalog.Application.Categories.Queries
{
    public record GetAllCategoriesRequest() : IRequest<List<CategoryDto>>;

    public sealed class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesRequest, List<CategoryDto>>
    {
        private readonly ICategoryService categoryService;
        public GetAllCategoriesHandler(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesRequest request, CancellationToken cancellationToken)
        {
            return await categoryService.GetAllCategoriesAsync();
        }
    }
}
