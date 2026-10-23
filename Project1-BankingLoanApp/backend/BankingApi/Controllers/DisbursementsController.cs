using BankingApi.DTOs;
using BankingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/disbursements")]
[Authorize(Roles = "Admin")]
public class DisbursementsController : ControllerBase
{
    private readonly IDisbursementService _disbursements;

    public DisbursementsController(IDisbursementService disbursements)
    {
        _disbursements = disbursements;
    }

    [HttpGet]
    public async Task<ActionResult<List<DisbursementDto>>> GetAll() =>
        Ok(await _disbursements.GetAllAsync());

    [HttpPost("{applicationId:int}")]
    [ProducesResponseType(typeof(DisbursementDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DisbursementDto>> Disburse(int applicationId)
    {
        DisbursementDto disbursement = await _disbursements.DisburseAsync(applicationId);
        return StatusCode(StatusCodes.Status201Created, disbursement);
    }
}
