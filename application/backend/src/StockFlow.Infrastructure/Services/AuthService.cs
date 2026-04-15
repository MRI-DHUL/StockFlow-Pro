using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IRepository<RefreshToken> refreshTokenRepository,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User registration attempt for email: {Email}", registerDto.Email);

        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: User already exists with email: {Email}", registerDto.Email);
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            EmailConfirmed = true // Auto-confirm for now
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            _logger.LogError("User registration failed for {Email}. Errors: {Errors}", 
                registerDto.Email, 
                string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Assign default role
        await _userManager.AddToRoleAsync(user, "Staff");
        
        _logger.LogInformation("User registered successfully: {UserId}, {Email}", user.Id, user.Email);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            _logger.LogWarning("Login failed: Invalid credentials for email: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: Inactive account for email: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("User account is inactive");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User logged in successfully: {UserId}, {Email}", user.Id, user.Email);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refresh token attempt");

        var tokenEntity = (await _refreshTokenRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(t => t.Token == refreshToken);

        if (tokenEntity == null || !tokenEntity.IsActive)
        {
            _logger.LogWarning("Refresh token failed: Invalid or expired token");
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _userManager.FindByIdAsync(tokenEntity.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Refresh token failed: User not found or inactive, UserId: {UserId}", tokenEntity.UserId);
            throw new UnauthorizedAccessException("User not found or inactive");
        }

        // Revoke old token
        tokenEntity.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(tokenEntity, cancellationToken);

        _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Token revocation attempt");

        var tokenEntity = (await _refreshTokenRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(t => t.Token == refreshToken);

        if (tokenEntity == null)
        {
            _logger.LogWarning("Token revocation failed: Token not found");
            return false;
        }

        tokenEntity.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(tokenEntity, cancellationToken);

        _logger.LogInformation("Token revoked successfully for user: {UserId}", tokenEntity.UserId);

        return true;
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshTokenString = _tokenService.GenerateRefreshToken();
        var expiresAt = _tokenService.GetTokenExpiration();

        // Save refresh token
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7) // Refresh token valid for 7 days
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshTokenString,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            }
        };
    }
}
