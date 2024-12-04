using Auth.API.Services;
using MediatR;
using Auth.API.Domain.Dtos;
using Security.Models;

namespace Auth.API.Auth
{
    public record RegisterUserRequest(CoffeeShopUserDto UserDto) : IRequest<IResult>;

    public class RegisterUserHandler(IAuthService authService) : IRequestHandler<RegisterUserRequest, IResult>
    {
        public async Task<IResult> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var (result, userId) = await authService.RegisterUserAsync(request.UserDto, Roles.USER);

            if (!result.Succeeded || string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest(result.Errors); 
            }

            return Results.Ok(result);
        }
    }
}
