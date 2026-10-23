using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Models;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

// GET /api/portfolio  -> everything the home page needs in one call
[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioService _service;
    public PortfolioController(IPortfolioService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PortfolioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PortfolioDto>> Get()
    {
        var portfolio = await _service.GetPortfolioAsync();
        return portfolio is null ? NotFound() : Ok(portfolio);
    }
}

// GET /api/experiences?type=Primary   GET /api/experiences/3
[ApiController]
[Route("api/[controller]")]
public class ExperiencesController : ControllerBase
{
    private readonly IPortfolioService _service;
    public ExperiencesController(IPortfolioService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExperienceDto>>> GetAll([FromQuery] ExperienceType? type) =>
        Ok(await _service.GetExperiencesAsync(type));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExperienceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExperienceDto>> GetById(int id)
    {
        var experience = await _service.GetExperienceAsync(id);
        return experience is null ? NotFound() : Ok(experience);
    }
}

// GET /api/skills?search=sql
[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly IPortfolioService _service;
    public SkillsController(IPortfolioService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SkillCategoryDto>>> Get([FromQuery] string? search) =>
        Ok(await _service.GetSkillsAsync(search));
}

// GET /api/projects?technology=React   GET /api/projects/technologies
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IPortfolioService _service;
    public ProjectsController(IPortfolioService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> Get([FromQuery] string? technology) =>
        Ok(await _service.GetProjectsAsync(technology));

    [HttpGet("technologies")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTechnologies() =>
        Ok(await _service.GetTechnologiesAsync());
}

// POST /api/contact  -> 201 Created, or 400 with validation errors
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IPortfolioService _service;
    public ContactController(IPortfolioService service) => _service = service;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateContactMessageDto dto)
    {
        // [ApiController] has already returned 400 if the data annotations failed
        var id = await _service.SaveContactMessageAsync(dto);
        return StatusCode(StatusCodes.Status201Created, new { id, message = "Thanks! Your message was received." });
    }
}
