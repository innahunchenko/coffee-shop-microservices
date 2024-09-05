using Catalog.Application.Categories.Queries;
using Catalog.Domain.Models.Dtos;
using Foundation.Abstractions.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Endpoints
{
    public class CategoriesController : ApiControllerBase
    {
        [HttpGet()]
        public Task<List<CategoryDto>> GetAllCategories([FromQuery] GetAllCategoriesRequest request, CancellationToken cancellationToken)
        {
            return Mediator.Send(request, cancellationToken);
        }
    }
}
