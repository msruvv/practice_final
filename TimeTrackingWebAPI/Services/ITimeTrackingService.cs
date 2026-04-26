using TimeTrackingWebAPI.DTO;

namespace TimeTrackingWebAPI.Services
{
    /// <summary>
    /// Сервис для работы с учетом рабочего времени.
    /// </summary>
    public interface ITimeTrackingService
    {
        /// <summary>
        /// Проверяет, не превысит ли добавление/обновление проводки лимит за день.
        /// </summary>
        Task<bool> ValidateDailyHoursLimitAsync(
            int taskId, DateTime date, decimal hours, int? excludeEntryId = null);

        /// <summary>
        /// Проверяет, можно ли редактировать задачу в проводке.
        /// </summary>
        Task<bool> CanEditTaskInTimeEntryAsync(int timeEntryId);

        /// <summary>
        /// Возвращает отчет за день с визуализацией.
        /// </summary>
        Task<TimeEntryReportDto> GetReportForDayAsync(DateTime date);

        /// <summary>
        /// Возвращает список проводок за неделю.
        /// </summary>
        Task<List<TimeEntryResponseDto>> GetReportForWeekAsync(DateTime date);

        /// <summary>
        /// Возвращает список проводок за месяц.
        /// </summary>
        Task<List<TimeEntryResponseDto>> GetReportForMonthAsync(int year, int month);
    }
}
