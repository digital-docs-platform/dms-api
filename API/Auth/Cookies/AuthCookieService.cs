
namespace API.Auth.Cookies
{
    public class AuthCookieService : IAuthCookieService
    {
        private readonly IConfiguration _config;

        public AuthCookieService(IConfiguration config)
        {
            _config = config;
        }

        public void SetAccessToken(HttpResponse response, string token)
        {
            var name = _config["AuthCookie:Name"] ?? "dms_at";

            var jwtMinutesStr = _config["Jwt:AccessTokenMinutes"];
            var jwtMinutes = int.TryParse(jwtMinutesStr, out var j) ? j : 15;

            var bufferMinutesStr = _config["AuthCookie:CookieBufferMinutes"];
            var bufferMinutes = int.TryParse(bufferMinutesStr, out var b) ? b : 5;

            var sameSite = ParseSameSite(_config["AuthCookie:SameSite"], SameSiteMode.Strict);
            var secure = ParseBool(_config["AuthCookie:Secure"], defaultValue: false);

            response.Cookies.Append(name, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = sameSite,
                Expires = DateTimeOffset.UtcNow.AddMinutes(jwtMinutes + bufferMinutes),
                // Path = "/" znači cookie važi za celu app
                Path = "/"
            });
        }

        public void ClearAccessToken(HttpResponse response)
        {
            var name = _config["AuthCookie:Name"] ?? "dms_at";
            var sameSite = ParseSameSite(_config["AuthCookie:SameSite"], SameSiteMode.Strict);
            var secure = ParseBool(_config["AuthCookie:Secure"], defaultValue: true);

            response.Cookies.Delete(name, new CookieOptions
            {
                Path = "/",
                SameSite = sameSite,
                Secure = secure
            });
        }



        private static SameSiteMode ParseSameSite(string? value, SameSiteMode fallback)
            => value?.ToLowerInvariant() switch
            {
                "none" => SameSiteMode.None,
                "lax" => SameSiteMode.Lax,
                "strict" => SameSiteMode.Strict,
                _ => fallback
            };

        private static bool ParseBool(string? value, bool defaultValue)
            => bool.TryParse(value, out var b) ? b : defaultValue;
    }
}
