using Microsoft.Extensions.Logging;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.BackgroundJobs;

public class TokenCleanupJobs
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TokenCleanupJobs> _logger;

    public TokenCleanupJobs(
        IRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<TokenCleanupJobs> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task CleanupExpiredTokens()
    {
        _logger.LogInformation("Starting expired refresh token cleanup...");

        try
        {
            var allTokens = await _refreshTokenRepository.GetAllAsync();
            var expiredTokens = allTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow || t.RevokedAt != null)
                .ToList();

            if (expiredTokens.Any())
            {
                foreach (var token in expiredTokens)
                {
                    await _refreshTokenRepository.DeleteAsync(token);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} expired/revoked refresh tokens", expiredTokens.Count);
            }
            else
            {
                _logger.LogInformation("No expired tokens to clean up");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cleaning up expired tokens");
            throw;
        }
    }
}
