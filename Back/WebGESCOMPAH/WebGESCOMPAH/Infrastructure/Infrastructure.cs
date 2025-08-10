using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.Extensions.Options;

namespace WebGESCOMPAH.Infrastructure
{
    public interface IAuthCookieFactory
    {
        CookieOptions AccessCookieOptions(DateTimeOffset expires);
        CookieOptions RefreshCookieOptions(DateTimeOffset expires);
        CookieOptions CsrfCookieOptions(DateTimeOffset expires);
    }

    public class AuthCookieFactory : IAuthCookieFactory
    {
        private readonly CookieSettings _settings;

        public AuthCookieFactory(IOptions<CookieSettings> cookieOptions)
        {
            _settings = cookieOptions.Value;
        }

        public CookieOptions AccessCookieOptions(DateTimeOffset expires) => new()
        {
            HttpOnly = true,
            Secure = _settings.Secure,
            SameSite = _settings.SameSite,
            Expires = expires.UtcDateTime,
            Path = _settings.Path,
            Domain = _settings.Domain
        };

        public CookieOptions RefreshCookieOptions(DateTimeOffset expires) => new()
        {
            HttpOnly = true,
            Secure = _settings.Secure,
            SameSite = SameSiteMode.None,
            Expires = expires.UtcDateTime,
            Path = _settings.Path,
            Domain = _settings.Domain
        };

        public CookieOptions CsrfCookieOptions(DateTimeOffset expires) => new()
        {
            HttpOnly = false, // accesible por JS para double-submit
            Secure = _settings.Secure,
            SameSite = SameSiteMode.None,
            Expires = expires.UtcDateTime,
            Path = _settings.Path,
            Domain = _settings.Domain
        };
    }
}
