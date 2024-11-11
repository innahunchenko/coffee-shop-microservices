using Auth.API.Domain.Dtos;
using Auth.API.Services;
using MediatR;

namespace Auth.API.Auth
{
    public record LoginUserRequest(CoffeeShopUserDto UserDto) : IRequest<IResult>;

    public class LoginUserHandler(IAuthService authService) : IRequestHandler<LoginUserRequest, IResult>
    {
        public async Task<IResult> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var result = await authService.LoginUserAsync(request.UserDto.UserName!, request.UserDto.Password!);
            return !result.Succeeded ? Results.BadRequest(result.Errors) : Results.Ok(result);
        }
    }
}
