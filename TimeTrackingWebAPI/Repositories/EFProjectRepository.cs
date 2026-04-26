using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Repositories
{
    /// <summary>
    /// Репозиторий для работы с проектами.
    /// </summary>
    public class EFProjectRepository : IProjectRepository
    {
        /// <summary>
        /// Контекст базы данных.
        /// </summary>
        private readonly TimeTrackingDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория проектов.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public EFProjectRepository(TimeTrackingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает список проектов.
        /// </summary>
        /// <param name="includeInactive">Включать неактивные.</param>
        /// <returns>Список проектов.</returns>
        public IEnumerable<Project> GetProjects(bool includeInactive = false)
        {
            var query = _context.Projects.AsQueryable();
            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            return query.ToList();
        }

        /// <summary>
        /// Возвращает проект по ID.
        /// </summary>
        /// <param name="id">ID проекта.</param>
        /// <returns>Проект или null.</returns>
        public Project? GetProjectById(int id)
        {
            return _context.Projects.Find(id);
        }

        /// <summary>
        /// Создает новый проект.
        /// </summary>
        /// <param name="project">Данные проекта.</param>
        public void CreateProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        /// <summary>
        /// Обновляет проект.
        /// </summary>
        /// <param name="project">Данные проекта.</param>
        public void UpdateProject(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }

        /// <summary>
        /// Удаляет проект.
        /// </summary>
        /// <param name="id">ID проекта.</param>
        /// <returns>Удаленный проект или null.</returns>
        public Project? DeleteProject(int id)
        {
            var project = GetProjectById(id);
            if (project != null)
            {
                var hasActiveTasks = _context.Tasks.Any(t => t.ProjectId == id && t.IsActive);
                if (hasActiveTasks)
                {
                    throw new InvalidOperationException(
                        "Нельзя удалить проект с активными задачами. " +
                        "Сначала деактивируйте все задачи проекта.");
                }

                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            return project;
        }
    }
}
