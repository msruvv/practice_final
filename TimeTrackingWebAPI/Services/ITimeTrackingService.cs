using TimeTrackingWebAPI.DTO;

namespace TimeTrackingWebAPI.Services
{
    public interface ITimeTrackingService
    {
        // Методы для проверки бизнес правил
        Task<bool> ValidateDailyHoursLimitAsync(int taskId, DateTime date, decimal hours, int? excludeEntryId = null);
        Task<bool> CanEditTaskInTimeEntryAsync(int timeEntryId);

        // Методы для отчетов
        Task<TimeEntryReportDto> GetReportForDayAsync(DateTime date);
        Task<TimeEntryReportDto> GetReportForWeekAsync(DateTime date);
        Task<TimeEntryReportDto> GetReportForMonthAsync(int year, int month);
    }
}
