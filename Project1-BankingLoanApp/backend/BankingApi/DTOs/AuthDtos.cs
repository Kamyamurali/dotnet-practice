using System.ComponentModel.DataAnnotations;

namespace BankingApi.DTOs;

public class RegisterRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? Phone { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters and include upper case, lower case, a number and a special character.")]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
}
