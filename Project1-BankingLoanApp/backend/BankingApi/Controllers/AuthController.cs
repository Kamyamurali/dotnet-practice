using BankingApi.DTOs;
using BankingApi.Security;
using BankingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        AuthResponse response = await _auth.RegisterAsync(request);
        return CreatedAtAction(nameof(Me), null, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request) =>
        Ok(await _auth.LoginAsync(request));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me() =>
        Ok(await _auth.GetProfileAsync(User.GetUserId()));
}
