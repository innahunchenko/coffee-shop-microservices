using Auth.API.Domain.Dtos;
using Auth.API.Services;
using MediatR;
using Security.Models;

namespace Auth.API.Auth
{
    public record RegisterAdminRequest(CoffeeShopUserDto UserDto) : IRequest<IResult>;

    public class RegisterAdminHandler(IAuthService authService) : IRequestHandler<RegisterAdminRequest, IResult>
    {
        public async Task<IResult> Handle(RegisterAdminRequest request, CancellationToken cancellationToken)
        {
            var (result, userId) = await authService.RegisterUserAsync(request.UserDto);

            if (!result.Succeeded || string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest(result.Errors);
            }

            result = await authService.AddUserToUserRoleAsync(userId, Roles.ADMIN);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(result);
        }
    }
}
