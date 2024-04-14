using System.Security.Claims;

namespace Domain.Interfaces.Services
{
    public interface IAuthService
    {
        string GenerateToken(int claimId, bool isHttpOnly, IEnumerable<Claim>? newClaims = null, DateTime? expirationDate = null);
    }
}
