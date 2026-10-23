using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Models;
using BankingApi.Repositories;
using BankingApi.Security;

namespace BankingApi.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserDto> GetProfileAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _tokens;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository users, IPasswordHasher hasher,
                       IJwtTokenService tokens, ILogger<AuthService> logger)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        string email = request.Email.Trim().ToLowerInvariant();

        if (await _users.GetByEmailAsync(email) is not null)
            throw new ConflictException("An account with this email already exists.");

        User user = await _users.RegisterAsync(request.FullName.Trim(), email,
                                               request.Phone?.Trim(), _hasher.Hash(request.Password));

        _logger.LogInformation("New customer registered: {UserId}", user.UserId);
        return BuildResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        User? user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());

        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for {Email}", request.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        return BuildResponse(user);
    }

    public async Task<UserDto> GetProfileAsync(int userId)
    {
        User user = await _users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");
        return ToDto(user);
    }

    private AuthResponse BuildResponse(User user)
    {
        var (token, expiresAt) = _tokens.CreateToken(user);
        return new AuthResponse { Token = token, ExpiresAt = expiresAt, User = ToDto(user) };
    }

    private static UserDto ToDto(User u) => new()
    {
        UserId = u.UserId,
        FullName = u.FullName,
        Email = u.Email,
        Phone = u.Phone,
        Role = u.Role,
        CreatedAt = u.CreatedAt
    };
}
