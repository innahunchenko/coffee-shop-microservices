namespace ShoppingCart.API.Services
{
    public interface ICookieService
    {
        string? GetDataFromCookies(HttpContext httpContext, string cookieName);
        void SetDataToCookies(HttpContext httpContext, string cookieName, string cookieValue);
    }
}
