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

            // Уникальный индекс по коду проекта
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // Уникальный индекс по проекту и названию задачи
            modelBuilder.Entity<Models.Task>()
                .HasIndex(t => new { t.ProjectId, t.Name })
                .IsUnique();

            // Составной индекс для поиска по дате и задаче
            modelBuilder.Entity<TimeEntry>()
                .HasIndex(te => new { te.Date, te.TaskId });
        }
    }
}
