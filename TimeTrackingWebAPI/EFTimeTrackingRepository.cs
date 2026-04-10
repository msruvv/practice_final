using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI
{
    /// <summary>
    /// Репозиторий для работы с данными
    /// </summary>
    public class EFTimeTrackingRepository : ITimeTrackingRepository
    {
        private readonly TimeTrackingDbContext _context;

        public EFTimeTrackingRepository(TimeTrackingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает список проектов
        /// </summary>
        /// <param name="includeInactive">Включать неактивные</param>
        /// <returns>Список проектов</returns>
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
        /// Возвращает проект по ID
        /// </summary>
        /// <param name="id">ID проекта</param>
        /// <returns>Проект или null</returns>
        public Project? GetProjectById(int id)
        {
            return _context.Projects.Find(id);
        }

        /// <summary>
        /// Создает новый проект
        /// </summary>
        /// <param name="project">Данные проекта</param>
        public void CreateProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        /// <summary>
        /// Обновляет проект
        /// </summary>
        /// <param name="project">Данные проекта</param>
        public void UpdateProject(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }

        /// <summary>
        /// Удаляет проект
        /// </summary>
        /// <param name="id">ID проекта</param>
        /// <returns>Удаленный проект или null</returns>
        public Project? DeleteProject(int id)
        {
            var project = GetProjectById(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            return project;
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
        /// Возвращает список проводок
        /// </summary>
        /// <param name="fromDate">Начальная дата</param>
        /// <param name="toDate">Конечная дата</param>
        /// <param name="taskId">ID задачи</param>
        /// <returns>Список проводок</returns>
        public IEnumerable<TimeEntry> GetTimeEntries(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? taskId = null)
        {
            var query = _context.TimeEntries.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(te => te.Date.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(te => te.Date.Date <= toDate.Value.Date);
            }

            if (taskId.HasValue)
            {
                query = query.Where(te => te.TaskId == taskId.Value);
            }

            return query.OrderByDescending(te => te.Date).ToList();
        }

        /// <summary>
        /// Возвращает проводку по ID
        /// </summary>
        /// <param name="id">ID проводки</param>
        /// <returns>Проводка или null</returns>
        public TimeEntry? GetTimeEntryById(int id)
        {
            return _context.TimeEntries.Find(id);
        }

        /// <summary>
        /// Создает новую проводку
        /// </summary>
        /// <param name="timeEntry">Данные проводки</param>
        public void CreateTimeEntry(TimeEntry timeEntry)
        {
            _context.TimeEntries.Add(timeEntry);
            _context.SaveChanges();
        }

        /// <summary>
        /// Обновляет проводку
        /// </summary>
        /// <param name="timeEntry">Данные проводки</param>
        public void UpdateTimeEntry(TimeEntry timeEntry)
        {
            _context.TimeEntries.Update(timeEntry);
            _context.SaveChanges();
        }

        /// <summary>
        /// Удаляет проводку
        /// </summary>
        /// <param name="id">ID проводки</param>
        /// <returns>Удаленная проводка или null</returns>
        public TimeEntry? DeleteTimeEntry(int id)
        {
            var timeEntry = GetTimeEntryById(id);
            if (timeEntry != null)
            {
                _context.TimeEntries.Remove(timeEntry);
                _context.SaveChanges();
            }
            return timeEntry;
        }

        /// <summary>
        /// Возвращает сумму часов за день
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="excludeEntryId">ID проводки для исключения</param>
        /// <returns>Сумма часов</returns>
        public decimal GetDailyHoursSum(DateTime date, int? excludeEntryId = null)
        {
            var query = _context.TimeEntries
                .Where(te => te.Date.Date == date.Date);

            if (excludeEntryId.HasValue)
            {
                query = query.Where(te => te.Id != excludeEntryId.Value);
            }

            return query.Sum(te => te.Hours);
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