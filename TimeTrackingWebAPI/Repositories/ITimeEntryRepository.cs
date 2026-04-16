using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Repositories
{
    /// <summary>
    /// Репозиторий для работы с проводками времени
    /// </summary>
    public interface ITimeEntryRepository
    {
        /// <summary>
        /// Возвращает список проводок
        /// </summary>
        IEnumerable<TimeEntry> GetTimeEntries(DateTime? fromDate = null,
            DateTime? toDate = null, int? taskId = null);

        /// <summary>
        /// Возвращает проводку по ID
        /// </summary>
        TimeEntry? GetTimeEntryById(int id);

        /// <summary>
        /// Создает новую проводку
        /// </summary>
        void CreateTimeEntry(TimeEntry timeEntry);

        /// <summary>
        /// Обновляет проводку
        /// </summary>
        void UpdateTimeEntry(TimeEntry timeEntry);

        /// <summary>
        /// Удаляет проводку
        /// </summary>
        TimeEntry? DeleteTimeEntry(int id);

        /// <summary>
        /// Возвращает сумму часов за день
        /// </summary>
        decimal GetDailyHoursSum(DateTime date, int? excludeEntryId = null);
    }
}
