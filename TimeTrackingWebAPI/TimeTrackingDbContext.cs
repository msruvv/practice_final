using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI
{
    /// <summary>
    /// Контекст базы данных для учета рабочего времени.
    /// </summary>
    public class TimeTrackingDbContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста БД.
        /// </summary>
        /// <param name="options">Настройки контекста.</param>
        public TimeTrackingDbContext(DbContextOptions<TimeTrackingDbContext> options)
            : base(options) { }

        /// <summary>
        /// Проекты.
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Задачи.
        /// </summary>
        public DbSet<Models.Task> Tasks { get; set; }

        /// <summary>
        /// Проводки.
        /// </summary>
        public DbSet<TimeEntry> TimeEntries { get; set; }

        /// <summary>
        /// Настройка модели БД.
        /// </summary>
        /// <param name="modelBuilder">Строитель модели.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройки проектов.
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(p => p.Code)
                    .IsUnique();

                entity.HasMany(p => p.Tasks)
                    .WithOne(t => t.Project)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройки задач.
            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(t => new { t.ProjectId, t.Name })
                    .IsUnique();

                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройки проводок.
            modelBuilder.Entity<TimeEntry>(entity =>
            {
                entity.Property(te => te.Description)
                    .HasMaxLength(500);

                entity.Property(te => te.Hours)
                    .HasPrecision(5, 2);

                entity.HasIndex(te => new { te.Date, te.TaskId });

                entity.HasOne(te => te.Task)
                    .WithMany(t => t.TimeEntries)
                    .HasForeignKey(te => te.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t => t.HasCheckConstraint("CK_Hours", "[Hours] >= 0.01 AND [Hours] <= 24"));
            });
        }
    }
}