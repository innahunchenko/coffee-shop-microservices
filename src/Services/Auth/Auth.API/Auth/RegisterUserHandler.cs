using Auth.API.Services;
using MediatR;
using Auth.API.Domain.Dtos;

namespace Auth.API.Auth
{
    public record RegisterUserRequest(CoffeeShopUserDto UserDto) : IRequest<IResult>;

    public class RegisterUserHandler(IAuthService authService) : IRequestHandler<RegisterUserRequest, IResult>
    {
        public async Task<IResult> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var (result, userId) = await authService.RegisterUserAsync(request.UserDto);

            if (result.Errors.Count() != 0) 
            {
                return Results.BadRequest(result.Errors);
            }

            if (result.Succeeded && !string.IsNullOrEmpty(userId)) 
            {
                result = await authService.AddUserToUserRoleAsync(userId);

                if (result.Succeeded)
                {
                    return Results.Ok(result);
                }
            }

            return Results.BadRequest(result.Errors);
        }
    }
}
