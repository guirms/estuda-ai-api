using Domain.Utils.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Utils.Helpers
{
    public static class HttpContextHelper
    {
        private static JwtSecurityToken? HeaderAuthToken { get; set; }

        public static int GetUserId()
            => HeaderAuthToken?.GetClaimValue(Token.NameIdentifier)?.ToInt("InvalidAuthToken")
                ?? throw new InvalidOperationException("ErrorGettingSessionInfo");

        public static string? GetClaimValueFromHeaderToken(string key) => HeaderAuthToken?.GetClaimValue(key);
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
