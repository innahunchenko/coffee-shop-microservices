namespace ShoppingCart.API.Services
{
    public class CookieService : ICookieService
    {
        public string? GetDataFromCookies(HttpContext httpContext, string cookieName)
        {
            if (httpContext.Request.Cookies.TryGetValue(cookieName, out var cartId))
            {
                return cartId;
            }

            return null;
        }

        public void SetDataToCookies(HttpContext httpContext, string cookieName, string cookieValue)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                HttpOnly = true,                        
                Secure = true,                          
                SameSite = SameSiteMode.Strict          
            };

            httpContext.Response.Cookies.Append(cookieName, cookieValue, cookieOptions);
        }
    }
}
