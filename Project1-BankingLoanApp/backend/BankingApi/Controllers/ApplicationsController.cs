using BankingApi.DTOs;
using BankingApi.Models;
using BankingApi.Security;
using BankingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]  
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applications;

    public ApplicationsController(IApplicationService applications)
    {
        _applications = applications;
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationDto>> Apply(ApplyLoanRequest request)
    {
        ApplicationDto application = await _applications.ApplyAsync(User.GetUserId(), request);
        return CreatedAtAction(nameof(GetById), new { id = application.ApplicationId }, application);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<ApplicationDto>>> GetMine() =>
        Ok(await _applications.GetMyApplicationsAsync(User.GetUserId()));

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<ApplicationDto>>> GetAll([FromQuery] string? status) =>
        Ok(await _applications.GetAllAsync(status));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(int id) =>
        Ok(await _applications.GetByIdAsync(id, User.GetUserId(), User.IsAdmin()));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<ApplicationDto>> Review(int id, ReviewApplicationRequest request) =>
        Ok(await _applications.ReviewAsync(id, request));

    [HttpGet("{id:int}/statement")]
    public async Task<ActionResult<LoanStatementDto>> GetStatement(int id) =>
        Ok(await _applications.GetStatementAsync(id, User.GetUserId(), User.IsAdmin()));
}
