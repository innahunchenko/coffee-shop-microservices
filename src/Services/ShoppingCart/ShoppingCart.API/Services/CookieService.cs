namespace ShoppingCart.API.Services
{
    public class CookieService : ICookieService
    {
        private const string CartIdCookieName = "cartId";

        public string? GetCartIdFromCookies(HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.TryGetValue(CartIdCookieName, out var cartId))
            {
                return cartId;
            }

            return null;
        }

        public void SetCartIdInCookies(HttpContext httpContext, string cartId)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                HttpOnly = true,                        
                Secure = true,                          
                SameSite = SameSiteMode.Strict          
            };

            httpContext.Response.Cookies.Append(CartIdCookieName, cartId, cookieOptions);
        }
    }
}
