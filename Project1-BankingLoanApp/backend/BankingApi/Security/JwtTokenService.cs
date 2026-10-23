using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankingApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankingApi.Security;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(AppClaims.UserId, user.UserId.ToString()),
            new(AppClaims.Name, user.FullName),
            new(AppClaims.Email, user.Email),
            new(AppClaims.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        DateTime expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}

public static class AppClaims
{
    public const string UserId = "sub";
    public const string Name = "name";
    public const string Email = "email";
    public const string Role = "role";
}

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(AppClaims.UserId)
                  ?? throw new UnauthorizedAccessException("User id missing from token."));

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");
}
