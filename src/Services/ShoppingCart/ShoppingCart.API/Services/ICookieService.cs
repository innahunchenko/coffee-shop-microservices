namespace ShoppingCart.API.Services
{
    public interface ICookieService
    {
        string? GetCartIdFromCookies(HttpContext httpContext);
        void SetCartIdInCookies(HttpContext httpContext, string cartId);
    }
}
