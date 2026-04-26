using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI
{
    /// <summary>
    /// Контекст базы данных для учета рабочего времени
    /// </summary>
    public class TimeTrackingDbContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста БД
        /// </summary>
        /// <param name="options">Настройки контекста</param>
        public TimeTrackingDbContext(DbContextOptions<TimeTrackingDbContext> options)
            : base(options) { }

        /// <summary>
        /// Проекты
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Задачи
        /// </summary>
        public DbSet<Models.Task> Tasks { get; set; }

        /// <summary>
        /// Проводки
        /// </summary>
        public DbSet<TimeEntry> TimeEntries { get; set; }

        /// <summary>
        /// Настройка модели БД
        /// </summary>
        /// <param name="modelBuilder">Строитель модели</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            // ==================== НАСТРОЙКИ ЗАДАЧ ====================
            modelBuilder.Entity<Models.Task>(entity =>
            {
                // Ограничения длины
                entity.Property(t => t.Name)
                    .IsRequired()           // NOT NULL
                    .HasMaxLength(200);     // NVARCHAR(200)

                // Уникальный индекс по проекту и названию задачи
                entity.HasIndex(t => new { t.ProjectId, t.Name })
                    .IsUnique();

                // Связь с проектом (каскадное удаление)
                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== НАСТРОЙКИ ПРОВОДОК ====================
            modelBuilder.Entity<TimeEntry>(entity =>
            {
                // Ограничения длины
                entity.Property(te => te.Description)
                    .HasMaxLength(500);     // NVARCHAR(500)

                // Точность для часов (decimal(5,2))
                entity.Property(te => te.Hours)
                    .HasPrecision(5, 2);

                // Составной индекс для поиска по дате и задаче
                entity.HasIndex(te => new { te.Date, te.TaskId });

                // Связь с задачей (каскадное удаление)
                entity.HasOne(te => te.Task)
                    .WithMany(t => t.TimeEntries)
                    .HasForeignKey(te => te.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Check-ограничение для часов (0.01 - 24)
                entity.ToTable(t => t.HasCheckConstraint("CK_Hours", "[Hours] >= 0.01 AND [Hours] <= 24"));
            });
        }
    }
}