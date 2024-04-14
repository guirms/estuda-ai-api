using Domain.Utils.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Domain.Utils.Helpers
{
    public static class HttpContextHelper
    {
        private static JwtSecurityToken? CookieAuthToken { get; set; }
        private static JwtSecurityToken? HeaderAuthToken { get; set; }

        public static int GetUserId()
            => CookieAuthToken?.GetClaimValue(Token.NameIdentifier)?.ToInt("InvalidAuthToken")
                ?? throw new InvalidOperationException("ErrorGettingSessionInfo");

        public static string GetRequestIpv4(this IHttpContextAccessor contextAccessor) => contextAccessor?.HttpContext?.Connection?
            .RemoteIpAddress?.ToString().ToValidIp() ?? throw new InvalidOperationException("ErrorGettingRequestIp");

        public static void ValidatePermission()
        {
            if (GetUserId() != 0)
                throw new InvalidOperationException("UserWithoutPermission");
        }

        public static string? GetClaimValueFromCookieToken(string key) => CookieAuthToken?.GetClaimValue(key);

        public static string? GetClaimValueFromHeaderToken(string key) => HeaderAuthToken?.GetClaimValue(key);

        public static bool IsValidAuthToken()
        {
            try
            {
                if (CookieAuthToken?.GetClaimValue(Token.ClaimPassword) != HeaderAuthToken?.GetClaimValue(Token.ClaimPassword))
                    return false;

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                tokenHandler.ValidateToken(CookieAuthToken?.RawData, validationParameters, out var validatedHttpOnlyAuthToken);
                tokenHandler.ValidateToken(HeaderAuthToken?.RawData, validationParameters, out var validatedHeaderAuthToken);

                return validatedHeaderAuthToken.ValidTo >= DateTime.UtcNow || validatedHttpOnlyAuthToken.ValidTo >= DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAssetPasswordCorrect() => HeaderAuthToken?.GetClaimValue(Token.ClaimPassword) == Pwd.Asset;

        public static void SaveTokens(this IHttpContextAccessor contextAccessor)
        {
            CookieAuthToken = contextAccessor.GetAuthTokenByCookie();
            HeaderAuthToken = contextAccessor.GetAuthTokenByHeader();
        }

        public static string? GetHeaderValue(this IHttpContextAccessor contextAccessor, string key)
        {
            if (contextAccessor.HttpContext == null)
                return null;

            contextAccessor.HttpContext.Request.Headers.TryGetValue(key, out StringValues headerValue);

            return headerValue;
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes($"{Pwd.Pf}_authSalt_token".ToSafeValue());

            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        }

        private static JwtSecurityToken? GetAuthTokenByCookie(this IHttpContextAccessor contextAccessor)
        {
            try
            {
                var stringToken = contextAccessor.HttpContext!.Request.Cookies.FirstOrDefault(c => c.Key == Token.TokenKey).Value
                    ?? throw new InvalidOperationException("ErrorGettingSessionInfo");

                return new JwtSecurityTokenHandler().ReadJwtToken(stringToken);
            }
            catch
            {
                return null;
            }
        }

        private static JwtSecurityToken? GetAuthTokenByHeader(this IHttpContextAccessor contextAccessor)
        {
            try
            {
                contextAccessor.HttpContext!.Request.Headers.TryGetValue(Token.Authorization, out StringValues headerValue);

                var stringToken = headerValue.FirstOrDefault()?.ToCleanJwtToken()
                    ?? throw new InvalidOperationException("ErrorGettingSessionInfo");

                return new JwtSecurityTokenHandler().ReadJwtToken(stringToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
