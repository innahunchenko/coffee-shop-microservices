using Auth.API.Domain.Dtos;
using Auth.API.Services;
using MediatR;

namespace Auth.API.Auth
{
    public record ResetPasswordRequest(ResetPasswordDto ResetPasswordDto) : IRequest<IResult>;
    public class ResetPasswordHandler(IAuthService authService) : IRequestHandler<ResetPasswordRequest, IResult>
    {
        public async Task<IResult> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var (result, user) = await authService.GetUserByEmailAsync(request.ResetPasswordDto.Email);

            if (user == null || !result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            result = await authService.ResetPasswordAsync(user, request.ResetPasswordDto.Token, request.ResetPasswordDto.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(result.Succeeded);
        }
    }
}
