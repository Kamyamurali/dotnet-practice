using BankingApi.Models;
using BankingApi.Security;
using BankingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _history;

    public HistoryController(IHistoryService history)
    {
        _history = history;
    }

    [HttpGet("transactions/my")]
    public async Task<ActionResult<List<Transaction>>> GetMyHistory([FromQuery] string? type) =>
        Ok(await _history.GetHistoryAsync(User.GetUserId(), type));

    [HttpGet("dashboard/customer")]
    public async Task<ActionResult<CustomerSummary>> GetCustomerSummary() =>
        Ok(await _history.GetCustomerSummaryAsync(User.GetUserId()));

    [Authorize(Roles = "Admin")]
    [HttpGet("dashboard/admin")]
    public async Task<ActionResult<AdminSummary>> GetAdminSummary() =>
        Ok(await _history.GetAdminSummaryAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("customers")]
    public async Task<ActionResult<List<CustomerInfo>>> GetCustomers() =>
        Ok(await _history.GetCustomersAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("customers/{userId:int}/transactions")]
    public async Task<ActionResult<List<Transaction>>> GetCustomerHistory(int userId) =>
        Ok(await _history.GetHistoryAsync(userId, null));
}
