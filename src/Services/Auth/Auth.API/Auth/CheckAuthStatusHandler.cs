using Auth.API.Services;
using MediatR;

namespace Auth.API.Auth
{
    public class CheckAuthStatusRequest : IRequest<bool> { }

    public class CheckAuthStatusHandler : IRequestHandler<CheckAuthStatusRequest, bool>
    {
        private readonly IJwtTokenService jwtTokenGenerator;

        public CheckAuthStatusHandler(IJwtTokenService jwtTokenGenerator)
        {
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        public Task<bool> Handle(CheckAuthStatusRequest request, CancellationToken cancellationToken)
        {
            var isValid = jwtTokenGenerator.ValidateCurrentJwtToken();
            return Task.FromResult(isValid);
        }
    }
}