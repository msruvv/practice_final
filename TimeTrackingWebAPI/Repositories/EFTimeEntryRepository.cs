using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Repositories
{
    /// <summary>
    /// Репозиторий для работы с задачами
    /// </summary>
    public class EFTimeEntryRepository : ITimeEntryRepository
    {
        /// <summary>
        /// Контекст базы данных
        /// </summary>
        private readonly TimeTrackingDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория проектов
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public EFTimeEntryRepository(TimeTrackingDbContext context)
        {
            _context = context;
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
