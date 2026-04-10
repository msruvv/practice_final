using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI
{
    /// <summary>
    /// Контекст базы данных для учета рабочего времени
    /// </summary>
    public class TimeTrackingDbContext : DbContext
    {
        public TimeTrackingDbContext(DbContextOptions<TimeTrackingDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<Models.Task>()
                .HasIndex(t => new { t.ProjectId, t.Name })
                .IsUnique();

            modelBuilder.Entity<TimeEntry>()
                .HasIndex(te => new { te.Date, te.TaskId });
        }
    }
}
