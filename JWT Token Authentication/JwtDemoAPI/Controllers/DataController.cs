using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public() =>
        Ok(new { message = "Public endpoint - anyone can see this." });

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile() => Ok(new
    {
        message = "Protected endpoint - your token is valid.",
        user = User.Identity?.Name,
        role = User.FindFirst("role")?.Value,
        claims = User.Claims.Select(c => new { c.Type, c.Value })
    });

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult Admin() =>
        Ok(new { message = $"Admin-only endpoint - welcome {User.Identity?.Name}." });
}