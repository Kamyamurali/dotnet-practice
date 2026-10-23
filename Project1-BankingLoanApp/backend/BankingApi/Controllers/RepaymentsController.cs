using BankingApi.DTOs;
using BankingApi.Models;
using BankingApi.Security;
using BankingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/repayments")]
[Authorize]
public class RepaymentsController : ControllerBase
{
    private readonly IRepaymentService _repayments;

    public RepaymentsController(IRepaymentService repayments)
    {
        _repayments = repayments;
    }

    [HttpGet("application/{applicationId:int}")]
    public async Task<ActionResult<List<Repayment>>> GetByApplication(int applicationId) =>
        Ok(await _repayments.GetByApplicationAsync(applicationId, User.GetUserId(), User.IsAdmin()));

    [Authorize(Roles = "Customer")]
    [HttpPost("{id:int}/pay")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaymentReceiptDto>> Pay(int id, PayInstallmentRequest request) =>
        Ok(await _repayments.PayAsync(id, User.GetUserId(), request.PaymentMethod));

    [HttpGet("application/{applicationId:int}/payoff-quote")]
    public async Task<ActionResult<PayoffQuoteDto>> GetPayoffQuote(int applicationId) =>
        Ok(await _repayments.GetPayoffQuoteAsync(applicationId, User.GetUserId(), User.IsAdmin()));

    [Authorize(Roles = "Customer")]
    [HttpPost("application/{applicationId:int}/payoff")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaymentReceiptDto>> PayOff(int applicationId, PayInstallmentRequest request) =>
        Ok(await _repayments.PayOffAsync(applicationId, User.GetUserId(), request.PaymentMethod));

    [HttpGet("receipts/{paymentId:int}")]
    public async Task<ActionResult<PaymentReceiptDto>> GetReceipt(int paymentId) =>
        Ok(await _repayments.GetReceiptAsync(paymentId, User.GetUserId(), User.IsAdmin()));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<Repayment>> UpdateStatus(int id, UpdateRepaymentStatusRequest request) =>
        Ok(await _repayments.UpdateStatusAsync(id, request.Status));
}