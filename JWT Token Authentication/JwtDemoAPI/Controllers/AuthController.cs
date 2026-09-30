using JwtDemoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokens;
    private readonly JwtKeys _keys;

    public AuthController(TokenService tokens, JwtKeys keys)
    {
        _tokens = tokens;
        _keys = keys;
    }

    private static readonly Dictionary<string, (string Password, string Role)> Users = new()
    {
        ["kamya"] = ("admin123", "Admin"),
        ["guest"] = ("guest123", "User")
    };

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request,
                               [FromQuery] string type = "hs256",
                               [FromQuery] bool useCookie = false)
    {
        if (!Users.TryGetValue(request.Username, out var user) || user.Password != request.Password)
            return Unauthorized(new { message = "Invalid username or password" });

        string token = type.ToLower() switch
        {
            "rs256" => _tokens.CreateRs256Token(request.Username, user.Role),
            "jwe"   => _tokens.CreateEncryptedToken(request.Username, user.Role),
            _       => _tokens.CreateHs256Token(request.Username, user.Role)
        };

        if (useCookie)
        {
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,                
                Secure = true,                 
                SameSite = SameSiteMode.Strict,  
                Expires = DateTimeOffset.UtcNow.AddMinutes(_tokens.ExpiryMinutes)
            });
            return Ok(new { type, storedIn = "HttpOnly cookie", message = "Token is in a cookie that JavaScript cannot read." });
        }

        return Ok(new { type, token, expiresInMinutes = _tokens.ExpiryMinutes });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return Ok(new { message = "Cookie cleared. (A copied JWT stays valid until it expires - JWT drawback!)" });
    }

    [HttpGet("public-key")]
    public IActionResult PublicKey()
    {
        var p = _keys.RsaSigningKey.Rsa.ExportParameters(false); // false = public part only
        return Ok(new
        {
            kty = "RSA",
            kid = _keys.RsaSigningKey.KeyId,
            alg = "RS256",
            n = Base64UrlEncoder.Encode(p.Modulus),
            e = Base64UrlEncoder.Encode(p.Exponent)
        });
    }
}

public record LoginRequest(string Username, string Password);