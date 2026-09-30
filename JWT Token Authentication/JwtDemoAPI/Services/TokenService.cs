using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemoAPI.Services;

public class TokenService
{
    private readonly JwtKeys _keys;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _minutes;

    public TokenService(JwtKeys keys, IConfiguration config)
    {
        _keys = keys;
        _issuer = config["Jwt:Issuer"]!;
        _audience = config["Jwt:Audience"]!;
        _minutes = config.GetValue<int>("Jwt:ExpiryMinutes");
    }

    public int ExpiryMinutes => _minutes;

    public string CreateHs256Token(string username, string role)
    {
        var claims = new[]
        {
            new Claim("sub", username),
            new Claim("name", username),
            new Claim("role", role),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(_keys.HmacKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

 
    public string CreateRs256Token(string username, string role)
    {
        var descriptor = BaseDescriptor(username, role);
        descriptor.SigningCredentials = new SigningCredentials(_keys.RsaSigningKey, SecurityAlgorithms.RsaSha256);
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public string CreateEncryptedToken(string username, string role)
    {
        var descriptor = BaseDescriptor(username, role);
        descriptor.SigningCredentials = new SigningCredentials(_keys.HmacKey, SecurityAlgorithms.HmacSha256);
        descriptor.EncryptingCredentials = new EncryptingCredentials(
            _keys.RsaEncryptionKey,
            SecurityAlgorithms.RsaOAEP,
            SecurityAlgorithms.Aes256CbcHmacSha512);
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    private SecurityTokenDescriptor BaseDescriptor(string username, string role) => new()
    {
        Issuer = _issuer,
        Audience = _audience,
        Expires = DateTime.UtcNow.AddMinutes(_minutes),
        Claims = new Dictionary<string, object>
        {
            ["sub"] = username,
            ["name"] = username,
            ["role"] = role,
            ["jti"] = Guid.NewGuid().ToString()
        }
    };
}