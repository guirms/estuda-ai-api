namespace Domain.Interfaces.Services
{
    public interface IAuthService
    {
        string GenerateToken(int claimId);
    }
}
