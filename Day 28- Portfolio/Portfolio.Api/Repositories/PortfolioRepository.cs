using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Models;

namespace Portfolio.Api.Repositories;

// Repository pattern: the only class that talks to EF Core.
// Interface Segregation / Dependency Inversion: controllers and services depend on the interface.
public interface IPortfolioRepository
{
    Task<Profile?> GetProfileAsync();
    Task<List<Education>> GetEducationAsync();
    Task<List<Experience>> GetExperiencesAsync(ExperienceType? type = null);
    Task<Experience?> GetExperienceByIdAsync(int id);
    Task<List<SkillCategory>> GetSkillCategoriesAsync();
    Task<List<Project>> GetProjectsAsync();
    Task<ContactMessage> AddContactMessageAsync(ContactMessage message);
}

public class PortfolioRepository : IPortfolioRepository
{
    private readonly PortfolioDbContext _db;

    public PortfolioRepository(PortfolioDbContext db) => _db = db;   // constructor injection

    public Task<Profile?> GetProfileAsync() =>
        _db.Profiles.AsNoTracking().FirstOrDefaultAsync();

    public Task<List<Education>> GetEducationAsync() =>
        _db.Educations.AsNoTracking().OrderByDescending(e => e.GraduationDate).ToListAsync();

    public Task<List<Experience>> GetExperiencesAsync(ExperienceType? type = null)
    {
        IQueryable<Experience> query = _db.Experiences.AsNoTracking().Include(e => e.Projects);
        if (type.HasValue)
            query = query.Where(e => e.Type == type.Value);
        return query.OrderBy(e => e.SortOrder).ToListAsync();
    }

    public Task<Experience?> GetExperienceByIdAsync(int id) =>
        _db.Experiences.AsNoTracking().Include(e => e.Projects).FirstOrDefaultAsync(e => e.Id == id);

    public Task<List<SkillCategory>> GetSkillCategoriesAsync() =>
        _db.SkillCategories.AsNoTracking()
            .Include(c => c.Skills)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

    public Task<List<Project>> GetProjectsAsync() =>
        _db.Projects.AsNoTracking()
            .Include(p => p.Experience)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();

    public async Task<ContactMessage> AddContactMessageAsync(ContactMessage message)
    {
        _db.ContactMessages.Add(message);
        await _db.SaveChangesAsync();
        return message;
    }
}
