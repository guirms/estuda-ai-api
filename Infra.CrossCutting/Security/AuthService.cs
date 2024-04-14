using Domain.Interfaces.Services;
using Domain.Utils.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infra.CrossCutting.Security
{
    public class AuthService(IConfiguration configuration) : IAuthService
    {
        public string GenerateToken(int claimId)
        {
            var jwtKey = configuration["Jwt:Key"].ToSafeValue();
            var jwtIssuer = configuration["Jwt:Issuer"].ToSafeValue();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, claimId.ToString())
            };

            var securityToken = new JwtSecurityToken(
                jwtIssuer,
                jwtIssuer,
                claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
