using Microsoft.EntityFrameworkCore;

namespace TimeTrackingWebAPI.Repositories
{
    /// <summary>
    /// Репозиторий для работы с задачами
    /// </summary>
    public class EFTaskRepository : ITaskRepository
    {
        /// <summary>
        /// Контекст базы данных
        /// </summary>
        private readonly TimeTrackingDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория проектов
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public EFTaskRepository(TimeTrackingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает список задач
        /// </summary>
        /// <param name="projectId">ID проекта (опционально)</param>
        /// <param name="includeInactive">Включать неактивные</param>
        /// <returns>Список задач</returns>
        public IEnumerable<Models.Task> GetTasks(
            int? projectId = null,
            bool includeInactive = false)
        {
            var query = _context.Tasks.AsQueryable();

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            if (!includeInactive)
                query = query.Where(t => t.IsActive);

            return query.ToList();
        }

        /// <summary>
        /// Возвращает задачу по ID
        /// </summary>
        /// <param name="id">ID задачи</param>
        /// <returns>Задача или null</returns>
        public Models.Task? GetTaskById(int id)
        {
            return _context.Tasks.Find(id);
        }

        /// <summary>
        /// Создает новую задачу
        /// </summary>
        /// <param name="task">Данные задачи</param>
        public void CreateTask(Models.Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        /// <summary>
        /// Обновляет задачу
        /// </summary>
        /// <param name="task">Данные задачи</param>
        public void UpdateTask(Models.Task task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }

        /// <summary>
        /// Удаляет задачу
        /// </summary>
        /// <param name="id">ID задачи</param>
        /// <returns>Удаленная задача или null</returns>
        /// <exception cref="InvalidOperationException">Если есть проводки по задаче</exception>
        public Models.Task? DeleteTask(int id)
        {
            var task = GetTaskById(id);
            if (task != null)
            {
                var hasEntries = _context.TimeEntries.Any(te => te.TaskId == id);
                if (hasEntries)
                    throw new InvalidOperationException("Нельзя удалить задачу, по которой есть списанные часы");

                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
            return task;
        }

        /// <summary>
        /// Проверяет, можно ли редактировать задачу в проводке
        /// </summary>
        /// <param name="timeEntryId">ID проводки</param>
        /// <returns>True - можно, False - нельзя</returns>
        public bool CanEditTaskInTimeEntry(int timeEntryId)
        {
            var isTaskActive = _context.TimeEntries
                .Where(te => te.Id == timeEntryId)
                .Join(_context.Tasks,
                    te => te.TaskId,
                    t => t.Id,
                    (te, t) => t.IsActive)
                .FirstOrDefault();

            return isTaskActive;
        }
    }
}
