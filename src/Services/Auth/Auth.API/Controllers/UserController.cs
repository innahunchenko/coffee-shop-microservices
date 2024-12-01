using Auth.API.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Auth.API.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Foundation.Abstractions.Services;
using Auth.API.Services;
using Security.Services;

namespace Auth.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISender sender;
        private readonly ICookieService cookieService;
        private readonly MenuService menuService;
        private readonly IUserContext userContext;
        private readonly string tokenCookieKey = "jwt-token";

        public UserController(ISender sender, 
            ICookieService cookieService, 
            MenuService menuService, 
            IUserContext userContext)
        {
            this.sender = sender;
            this.cookieService = cookieService;
            this.menuService = menuService;
            this.userContext = userContext;
        }

        [HttpPost("register")]
        public async Task<IResult> RegisterUser([FromBody] CoffeeShopUserDto request, CancellationToken ct)
        {
            var result = await sender.Send(new RegisterUserRequest(request), ct);
            return result;
        }

        [HttpGet("register-admin")]
        public async Task<IResult> RegisterAdmin(CancellationToken ct)
        {
            var request = new CoffeeShopUserDto()
            {
                Email = "admin@gmail.com",
                PhoneNumber = "234234235",
                UserName = "admin",
                Password = "AdminPass!23"
            };

            var result = await sender.Send(new RegisterAdminRequest(request), ct);
            return result;
        }

        [HttpPost("login")]
        public async Task<IResult> LoginUser([FromBody] CoffeeShopUserDto request, CancellationToken ct)
        {
            var result = await sender.Send(new LoginUserRequest(request), ct);
            return result;
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            cookieService.ClearData(tokenCookieKey);
            return Ok(true);
        }

        [HttpGet("check-auth-status")]
        [Authorize]
        public IActionResult CheckAuthenticationStatus(CancellationToken ct)
        {
            var isLoggedInUser = User?.Identity?.IsAuthenticated ?? false;
            return Ok(isLoggedInUser);
        }

        [HttpGet("is-user-admin")]
        [Authorize]
        public IActionResult IsUserAdmin(CancellationToken ct)
        {
            var userRole = userContext.GetUserRole();
            var isUserAdmin = userRole == Security.Models.Roles.ADMIN ? true : false;
            return Ok(isUserAdmin);
        }

        [HttpGet("role")]
        [Authorize]
        public IActionResult GetUserRole(CancellationToken ct)
        {
            var userRole = userContext.GetUserRole();
            return Ok(new { role = userRole.ToString() });
        }

        [Authorize]
        [HttpGet("username")]
        public IActionResult GetUserName(CancellationToken ct)
        {
            var username = userContext.GetUserName();
            return Ok(new { Username = username });
        }

        [Authorize]
        [HttpGet("menu")]
        public IActionResult GetUserMenu()
        {
            var menu = menuService.GetUserMenu();
            return Ok(menu);
        }
    }
}
