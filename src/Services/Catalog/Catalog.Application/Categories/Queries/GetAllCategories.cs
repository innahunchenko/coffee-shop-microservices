using Catalog.Domain.Models.Dtos;
using MediatR;
using Catalog.Domain.Services.Categories;

namespace Catalog.Application.Categories.Queries
{
    public record GetAllRequest() : IRequest<List<CategoryDto>>;

    public sealed class GetAllHandler : IRequestHandler<GetAllRequest, List<CategoryDto>>
    {
        private readonly ICategoryService categoryService;
        public GetAllHandler(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<List<CategoryDto>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return await categoryService.GetAsync();
        }
    }
}
