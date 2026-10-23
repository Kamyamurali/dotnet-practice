using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Models;

// Abstraction + inheritance: every table gets an Id from one base class.
public abstract class BaseEntity
{
    public int Id { get; set; }
}

public class Profile : BaseEntity
{
    [Required, MaxLength(100)] public string FullName { get; set; } = "";
    [Required, MaxLength(100)] public string Title { get; set; } = "";
    [MaxLength(200)] public string Tagline { get; set; } = "";
    [MaxLength(200)] public string CertificationNote { get; set; } = "";
    public List<string> AboutParagraphs { get; set; } = new();   // EF Core primitive collection (stored as JSON)
    [MaxLength(150)] public string? Email { get; set; }
    [MaxLength(200)] public string? GitHubUrl { get; set; }
    [MaxLength(200)] public string? LinkedInUrl { get; set; }
}

public class Education : BaseEntity
{
    [Required, MaxLength(150)] public string Degree { get; set; } = "";
    [Required, MaxLength(150)] public string Institution { get; set; } = "";
    [MaxLength(60)] public string Country { get; set; } = "";
    public DateOnly GraduationDate { get; set; }
    public decimal Gpa { get; set; }
}

public enum ExperienceType
{
    Primary = 1,   // "Work Experiences" on the Revature portfolio
    Other = 2      // "Other Experiences"
}

// One-to-many: an Experience (company + role) has many Projects.
public class Experience : BaseEntity
{
    [Required, MaxLength(100)] public string Company { get; set; } = "";
    [Required, MaxLength(100)] public string Role { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public ExperienceType Type { get; set; }
    public int SortOrder { get; set; }
    public List<Project> Projects { get; set; } = new();
}

public class Project : BaseEntity
{
    public int ExperienceId { get; set; }                 // foreign key
    public Experience? Experience { get; set; }           // navigation property
    [Required, MaxLength(150)] public string Title { get; set; } = "";
    [MaxLength(100)] public string Role { get; set; } = "";
    public string Description { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool Featured { get; set; }
    public List<string> Responsibilities { get; set; } = new();
    public List<string> Technologies { get; set; } = new();
}

public class SkillCategory : BaseEntity
{
    [Required, MaxLength(80)] public string Name { get; set; } = "";
    public int SortOrder { get; set; }
    public List<Skill> Skills { get; set; } = new();
}

public class Skill : BaseEntity
{
    public int SkillCategoryId { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    [Required, MaxLength(80)] public string Name { get; set; } = "";
    public int SortOrder { get; set; }
}

// Written by the contact form; read later from Azure SQL in VS Code (mssql extension).
public class ContactMessage : BaseEntity
{
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    [Required, MaxLength(150)] public string Email { get; set; } = "";
    [Required, MaxLength(2000)] public string Message { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
}
