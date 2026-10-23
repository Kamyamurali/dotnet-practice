namespace BankingApi.DTOs;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
}
