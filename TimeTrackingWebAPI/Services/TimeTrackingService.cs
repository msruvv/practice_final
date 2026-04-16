using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;
using TimeTrackingWebAPI.Models.Enums;

namespace TimeTrackingWebAPI.Services
{
    /// <summary>
    /// Сервис для работы с учетом рабочего времени
    /// </summary>
    public class TimeTrackingService : ITimeTrackingService
    {
        /// <summary>
        /// Количество часов в дне
        /// </summary>
        private const int HoursInDay = 24;

        /// <summary>
        /// Норма рабочих часов в день
        /// </summary>
        private const int DailyNormHours = 8;

        /// <summary>
        /// Количество дней в неделе
        /// </summary>
        private const int DaysInWeek = 7;

        /// <summary>
        /// Количество месяцев для добавления
        /// </summary>
        private const int MonthsToAdd = 1;

        /// <summary>
        /// Контекст базы данных
        /// </summary>
        private readonly TimeTrackingDbContext _context;

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public TimeTrackingService(TimeTrackingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Проверяет, не превысит ли добавление/обновление проводки лимит за день
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="date">Дата проводки</param>
        /// <param name="hours">Количество часов</param>
        /// <param name="excludeEntryId">ID проводки для исключения</param>
        /// <returns>True - лимит не превышен, False - лимит будет превышен</returns>
        public async Task<bool> ValidateDailyHoursLimitAsync(
            int taskId, DateTime date, decimal hours, int? excludeEntryId = null)
        {
            var query = _context.TimeEntries
                .Where(te => te.Date.Date == date.Date);

            if (excludeEntryId.HasValue)
            {
                query = query.Where(te => te.Id != excludeEntryId.Value);
            }

            var existingTotal = await query.SumAsync(te => te.Hours);

            return existingTotal + hours <= HoursInDay;
        }

        /// <summary>
        /// Проверяет, можно ли редактировать задачу в проводке
        /// </summary>
        /// <param name="timeEntryId">ID проводки</param>
        /// <returns>True - можно редактировать, False - нельзя</returns>
        public async Task<bool> CanEditTaskInTimeEntryAsync(int timeEntryId)
        {
            var timeEntry = await _context.TimeEntries
                .Include(te => te.Task)
                .FirstOrDefaultAsync(te => te.Id == timeEntryId);

            if (timeEntry == null || timeEntry.Task == null)
            {
                return false;
            }

            return timeEntry.Task.IsActive;
        }

        /// <summary>
        /// Возвращает отчет за день с визуализацией
        /// </summary>
        /// <param name="date">Дата отчета</param>
        /// <returns>Отчет с суммой часов и цветом стикера</returns>
        public async Task<TimeEntryReportDto> GetReportForDayAsync(DateTime date)
        {
            var entries = await _context.TimeEntries
                .Include(te => te.Task)
                    .ThenInclude(t => t!.Project)
                .Where(te => te.Date.Date == date.Date)
                .OrderBy(te => te.Date)
                .ToListAsync();

            var totalHours = entries.Sum(e => e.Hours);
            var status = totalHours < DailyNormHours
                ? ReportStatus.Under
                : (totalHours == DailyNormHours
                    ? ReportStatus.Normal
                    : ReportStatus.Over);
            var stickerColor = totalHours < DailyNormHours
                ? StickerColor.Yellow
                : (totalHours == DailyNormHours
                    ? StickerColor.Green
                    : StickerColor.Red);

            return new TimeEntryReportDto
            {
                Date = date,
                TotalHours = totalHours,
                Status = status,
                StickerColor = stickerColor,
                Entries = entries.Select(MapToResponseDto).ToList()
            };
        }

        /// <summary>
        /// Возвращает список проводок за неделю
        /// </summary>
        /// <param name="date">Любая дата в неделе</param>
        /// <returns>Список проводок за неделю</returns>
        public async Task<List<TimeEntryResponseDto>> GetReportForWeekAsync(DateTime date)
        {
            var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(DaysInWeek);

            var entries = await _context.TimeEntries
                .Include(te => te.Task)
                    .ThenInclude(t => t!.Project)
                .Where(te => te.Date.Date >= startOfWeek && te.Date.Date < endOfWeek)
                .OrderBy(te => te.Date)
                .ToListAsync();

            return entries.Select(MapToResponseDto).ToList();
        }

        /// <summary>
        /// Возвращает список проводок за месяц
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <returns>Список проводок за месяц</returns>
        public async Task<List<TimeEntryResponseDto>> GetReportForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(MonthsToAdd);

            var entries = await _context.TimeEntries
                .Include(te => te.Task)
                    .ThenInclude(t => t!.Project)
                .Where(te => te.Date.Date >= startDate && te.Date.Date < endDate)
                .OrderBy(te => te.Date)
                .ToListAsync();

            return entries.Select(MapToResponseDto).ToList();
        }

        private TimeEntryResponseDto MapToResponseDto(TimeEntry entry)
        {
            return new TimeEntryResponseDto
            {
                Id = entry.Id,
                Date = entry.Date,
                Hours = entry.Hours,
                Description = entry.Description,
                TaskId = entry.TaskId,
                TaskName = entry.Task?.Name ?? string.Empty,
                ProjectName = entry.Task?.Project?.Name ?? string.Empty,
                CanEditTask = entry.Task?.IsActive ?? false
            };
        }
    }
}