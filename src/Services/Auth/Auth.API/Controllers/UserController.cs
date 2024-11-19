using Auth.API.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Auth.API.Auth;
using System.Security.Claims;
using Auth.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Auth.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISender sender;
        private readonly IJwtTokenService jwtTokenService;

        public UserController(ISender sender, IJwtTokenService jwtTokenService)
        {
            this.sender = sender;
            this.jwtTokenService = jwtTokenService;
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
            var result = await sender.Send(new LoginUserRequest(request), ct);
            return result;
        }

        [HttpGet("check-auth-status")]
        public async Task<bool> CheckAuthenticationStatus(CancellationToken ct)
        {
            var isAuthenticated = await sender.Send(new CheckAuthStatusRequest(), ct);
            return isAuthenticated;
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize]
        [HttpGet("username")]
        public IActionResult GetUserName(CancellationToken ct)
        {
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var username = jwtTokenService.GetUsernameFromToken();

            //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            //var handler = new JwtSecurityTokenHandler();
            //var jwtToken = handler.ReadJwtToken(token);
            //var username = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            return Ok(new { Username = username });
        }

        [HttpGet("user-menu")]
        public IActionResult GetUserMenu()
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var menuItems = role switch
            {
                "admin" => new List<string> {
                "Admin panel",
                "Profile",
                "Sing out"
            },
                "user" => new List<string> {
                "Orders",
                "Profile",
                "Sing out"
            },
                _ => new List<string>()
            };

            return Ok(menuItems);
        }
    }
}
