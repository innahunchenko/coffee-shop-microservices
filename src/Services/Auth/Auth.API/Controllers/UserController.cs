using Auth.API.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Auth.API.Auth;

namespace Auth.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISender sender;

        public UserController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpPost("register")]
        public async Task<IResult> RegisterUser([FromBody] CoffeeShopUserDto request, CancellationToken ct)
        {
            var result = await sender.Send(new RegisterUserRequest(request), ct);
            return result;
        }

        [HttpPost("login")]
        public async Task<IResult> LoginUser([FromBody] CoffeeShopUserDto request, CancellationToken ct)
        {
            return Results.Ok();
            //var result = await sender.Send(new LoginUserRequest(request), ct);
            //return result;
        }
    }
}
