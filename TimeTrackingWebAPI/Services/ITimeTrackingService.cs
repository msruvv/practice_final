using TimeTrackingWebAPI.DTO;

namespace TimeTrackingWebAPI.Services
{
    /// <summary>
    /// Сервис для работы с учетом рабочего времени
    /// </summary>
    public interface ITimeTrackingService
    {
        // Методы для проверки бизнес правил
        Task<bool> ValidateDailyHoursLimitAsync(int taskId, DateTime date, decimal hours, int? excludeEntryId = null);
        Task<bool> CanEditTaskInTimeEntryAsync(int timeEntryId);

        // Методы для отчетов
        Task<TimeEntryReportDto> GetReportForDayAsync(DateTime date);
        Task<List<TimeEntryResponseDto>> GetReportForWeekAsync(DateTime date);
        Task<List<TimeEntryResponseDto>> GetReportForMonthAsync(int year, int month);
    }
}
