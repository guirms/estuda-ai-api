using Domain.Interfaces.Services;
using Domain.Utils.Constants;
using Domain.Utils.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infra.CrossCutting.Security
{
    public class AuthService(IHttpContextAccessor contextAccessor) : IAuthService
    {
        public string GenerateToken(int claimId, bool isHttpOnly, IEnumerable<Claim>? newClaims = null, DateTime? expirationDate = null)
        {
            var key = Encoding.ASCII.GetBytes($"{Pwd.Pf}_authSalt_token".ToSafeValue());

            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, claimId.ToString()),
            });

            if (newClaims != null)
                claims.AddClaims(newClaims);

            if (!expirationDate.HasValue)
                expirationDate = DateTime.UtcNow.AddHours(10);

            var tokenDescrIptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = expirationDate,
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescrIptor);

            var authToken = tokenHandler.WriteToken(token);

            if (isHttpOnly)
                EnableHttpOnlyToken(authToken, expirationDate.Value);

            return authToken;
        }

        private void EnableHttpOnlyToken(string userToken, DateTime expirationDate)
        {
            contextAccessor.HttpContext?.Response.Cookies.Append(Token.TokenKey, userToken,
                new CookieOptions
                {
                    Expires = expirationDate,
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true
                });
        }
    }
}
