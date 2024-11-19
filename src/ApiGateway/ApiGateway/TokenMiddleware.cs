using Foundation.Abstractions.Services;

namespace ApiGateway
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ICookieService cookieService;

        public TokenMiddleware(RequestDelegate next, ICookieService cookieService)
        {
            this.next = next;
            this.cookieService = cookieService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = cookieService.GetData("jwt-token");
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers["Authorization"] = $"Bearer {token}";
                Console.WriteLine($"Token added: {token}");
            }
            else
            {
                Console.WriteLine("No token found");
            }

            await next(context);
        }
    }
}
