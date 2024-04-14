using Domain.Utils.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Domain.Utils.Helpers
{
    public static class HttpContextHelper
    {
        private static JwtSecurityToken? HeaderAuthToken { get; set; }

        public static int GetUserId()
            => HeaderAuthToken?.GetClaimValue(Token.NameIdentifier)?.ToInt("InvalidAuthToken")
                ?? throw new InvalidOperationException("ErrorGettingSessionInfo");

        public static string? GetClaimValueFromHeaderToken(string key) => HeaderAuthToken?.GetClaimValue(key);

        public static bool IsValidAuthToken()
        {
            try
            {
                if (HeaderAuthToken?.GetClaimValue(Token.ClaimPassword) != Pwd.Auth)
                    return false;

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                tokenHandler.ValidateToken(HeaderAuthToken?.RawData, validationParameters, out var validatedHeaderAuthToken);

                return validatedHeaderAuthToken.ValidTo >= DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public static void SaveTokens(this IHttpContextAccessor contextAccessor)
        {
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
