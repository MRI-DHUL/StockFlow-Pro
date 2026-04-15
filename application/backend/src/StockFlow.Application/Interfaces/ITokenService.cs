namespace StockFlow.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    DateTime GetTokenExpiration();
}
