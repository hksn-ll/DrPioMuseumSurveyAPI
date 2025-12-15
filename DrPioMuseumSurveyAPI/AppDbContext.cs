using DrPioMuseumSurveyAPI.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SurveyResponse> Responses { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Admin> Admins { get; set; }
}