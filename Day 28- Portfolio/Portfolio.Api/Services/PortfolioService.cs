using System.Globalization;
using Portfolio.Api.DTOs;
using Portfolio.Api.Models;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

// Service layer: business logic + LINQ shaping. Single Responsibility:
// the repository fetches, the service shapes, the controller handles HTTP.
public interface IPortfolioService
{
    Task<PortfolioDto?> GetPortfolioAsync();
    Task<IReadOnlyList<ExperienceDto>> GetExperiencesAsync(ExperienceType? type);
    Task<ExperienceDto?> GetExperienceAsync(int id);
    Task<IReadOnlyList<SkillCategoryDto>> GetSkillsAsync(string? search);
    Task<IReadOnlyList<ProjectDto>> GetProjectsAsync(string? technology);
    Task<IReadOnlyList<string>> GetTechnologiesAsync();
    Task<int> SaveContactMessageAsync(CreateContactMessageDto dto);
}

public class PortfolioService : IPortfolioService
{
    private readonly IPortfolioRepository _repo;
    private readonly ILogger<PortfolioService> _logger;

    public PortfolioService(IPortfolioRepository repo, ILogger<PortfolioService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PortfolioDto?> GetPortfolioAsync()
    {
        var profile = await _repo.GetProfileAsync();
        if (profile is null) return null;

        var education = await _repo.GetEducationAsync();
        var experiences = await _repo.GetExperiencesAsync();
        var categories = await _repo.GetSkillCategoriesAsync();

        // LINQ: SelectMany flattens every project's technology list into one sequence
        var allProjects = experiences.SelectMany(e => e.Projects).ToList();
        var stats = new StatsDto(
            Companies: experiences.Select(e => e.Company).Distinct().Count(),
            Projects: allProjects.Count,
            Technologies: allProjects.SelectMany(p => p.Technologies)
                                     .Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            Skills: categories.Sum(c => c.Skills.Count));

        return new PortfolioDto(
            profile.ToDto(),
            education.Select(e => e.ToDto()).ToList(),
            experiences.Select(e => e.ToDto()).ToList(),
            categories.Select(c => c.ToDto()).ToList(),
            stats);
    }

    public async Task<IReadOnlyList<ExperienceDto>> GetExperiencesAsync(ExperienceType? type) =>
        (await _repo.GetExperiencesAsync(type)).Select(e => e.ToDto()).ToList();

    public async Task<ExperienceDto?> GetExperienceAsync(int id) =>
        (await _repo.GetExperienceByIdAsync(id))?.ToDto();

    public async Task<IReadOnlyList<SkillCategoryDto>> GetSkillsAsync(string? search)
    {
        var categories = (await _repo.GetSkillCategoriesAsync()).Select(c => c.ToDto());

        if (string.IsNullOrWhiteSpace(search))
            return categories.ToList();

        // Keep only matching skills, then drop categories left empty
        return categories
            .Select(c => c with { Skills = c.Skills.Where(s => s.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList() })
            .Where(c => c.Skills.Count > 0)
            .ToList();
    }

    public async Task<IReadOnlyList<ProjectDto>> GetProjectsAsync(string? technology)
    {
        var projects = await _repo.GetProjectsAsync();

        var filtered = string.IsNullOrWhiteSpace(technology)
            ? projects
            : projects.Where(p => p.Technologies.Any(t => t.Equals(technology, StringComparison.OrdinalIgnoreCase)));

        return filtered.Select(p => p.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<string>> GetTechnologiesAsync()
    {
        var projects = await _repo.GetProjectsAsync();

        // LINQ GroupBy: most-used technologies first
        return projects
            .SelectMany(p => p.Technologies)
            .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => g.Key)
            .ToList();
    }

    public async Task<int> SaveContactMessageAsync(CreateContactMessageDto dto)
    {
        var saved = await _repo.AddContactMessageAsync(new ContactMessage
        {
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim(),
            Message = dto.Message.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        });
        _logger.LogInformation("Contact message {Id} received from {Name}", saved.Id, saved.Name);
        return saved.Id;
    }
}

// Extension methods: entity -> DTO mapping in one place
public static class MappingExtensions
{
    private static string Month(DateOnly d) => d.ToString("MMM yyyy", CultureInfo.InvariantCulture);

    public static string Period(DateOnly start, DateOnly? end) =>
        end is null ? $"{Month(start)} - Present"
        : end.Value == start ? Month(start)
        : $"{Month(start)} - {Month(end.Value)}";

    public static ProfileDto ToDto(this Profile p) =>
        new(p.FullName, p.Title, p.Tagline, p.CertificationNote, p.AboutParagraphs, p.Email, p.GitHubUrl, p.LinkedInUrl);

    public static EducationDto ToDto(this Education e) =>
        new(e.Degree, e.Institution, e.Country, Month(e.GraduationDate), e.Gpa);

    public static ProjectDto ToDto(this Project p) =>
        new(p.Id, p.Title, p.Role, p.Description, Period(p.StartDate, p.EndDate), p.Featured,
            p.Responsibilities, p.Technologies);

    public static ExperienceDto ToDto(this Experience e) =>
        new(e.Id, e.Company, e.Role, e.Type.ToString(), Period(e.StartDate, e.EndDate),
            e.Projects.OrderByDescending(p => p.StartDate).ThenBy(p => p.Id).Select(p => p.ToDto()).ToList());

    public static SkillCategoryDto ToDto(this SkillCategory c) =>
        new(c.Name, c.Skills.OrderBy(s => s.SortOrder).Select(s => s.Name).ToList());
}
