using Microsoft.AspNetCore.Http;

namespace Foundation.Abstractions.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor contextAccessor;

        public CookieService(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public string? GetData(string key)
        {
            return contextAccessor.HttpContext?.Request.Cookies.TryGetValue(key, out var value) ?? false ? value : null;
        }

        public void SetData(string key, string value)
        {
            SetCookieData(key, value, null);
        }

        public void SetData(string key, string value, DateTimeOffset dateTimeOffset)
        {
            SetCookieData(key, value, dateTimeOffset);
        }

        private void SetCookieData(string key, string value, DateTimeOffset? dateTimeOffset)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = dateTimeOffset,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            contextAccessor.HttpContext?.Response.Cookies.Append(key, value, cookieOptions);
        }

        public void ClearData(string key)
        {
            contextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
