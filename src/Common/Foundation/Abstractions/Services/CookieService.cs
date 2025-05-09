﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

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

        public void ClearData(string key)
        {
            //contextAccessor.HttpContext?.Response.Cookies.Delete(key);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
             //   Domain = "https://api-gateway.thankfulhill-341e0e46.polandcentral.azurecontainerapps.io",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
                //Path = "/"
            };

            contextAccessor.HttpContext?.Response.Cookies.Append(key, "", cookieOptions);
        }

        private void SetCookieData(string key, string value, DateTimeOffset? dateTimeOffset)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = dateTimeOffset,
               // Domain = "https://api-gateway.thankfulhill-341e0e46.polandcentral.azurecontainerapps.io",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            contextAccessor.HttpContext?.Response.Cookies.Append(key, value, cookieOptions);
        }
    }
}
