using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Models;

namespace Portfolio.Api.Data;

// EF Core Code-First: these classes define the tables.
public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API: relationships and keys
        modelBuilder.Entity<Experience>()
            .HasMany(e => e.Projects)
            .WithOne(p => p.Experience)
            .HasForeignKey(p => p.ExperienceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SkillCategory>()
            .HasMany(c => c.Skills)
            .WithOne(s => s.SkillCategory)
            .HasForeignKey(s => s.SkillCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Experience>()
            .Property(e => e.Type)
            .HasConversion<string>()      // stores "Primary"/"Other" instead of 1/2
            .HasMaxLength(20);

        modelBuilder.Entity<Education>()
            .Property(e => e.Gpa)
            .HasPrecision(4, 2);

        modelBuilder.Entity<ContactMessage>()
            .HasIndex(m => m.CreatedAtUtc);
    }
}
