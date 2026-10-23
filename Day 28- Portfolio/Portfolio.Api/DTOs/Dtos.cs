using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.DTOs;

// DTOs (records) shape what the API returns, so entities never leak out directly.

public record ProfileDto(
    string FullName, string Title, string Tagline, string CertificationNote,
    IReadOnlyList<string> About, string? Email, string? GitHubUrl, string? LinkedInUrl);

public record EducationDto(string Degree, string Institution, string Country, string Graduated, decimal Gpa);

public record ProjectDto(
    int Id, string Title, string Role, string Description, string Period, bool Featured,
    IReadOnlyList<string> Responsibilities, IReadOnlyList<string> Technologies);

public record ExperienceDto(
    int Id, string Company, string Role, string Type, string Period, IReadOnlyList<ProjectDto> Projects);

public record SkillCategoryDto(string Name, IReadOnlyList<string> Skills);

public record StatsDto(int Companies, int Projects, int Technologies, int Skills);

public record PortfolioDto(
    ProfileDto Profile,
    IReadOnlyList<EducationDto> Education,
    IReadOnlyList<ExperienceDto> Experiences,
    IReadOnlyList<SkillCategoryDto> SkillCategories,
    StatsDto Stats);

// Input DTO: data annotations give automatic 400 Bad Request via [ApiController].
public class CreateContactMessageDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = "";

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Message { get; set; } = "";
}
