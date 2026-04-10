using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Services
{
    public class TimeTrackingService : ITimeTrackingService
    {
        private readonly TimeTrackingDbContext _context;

        public TimeTrackingService(TimeTrackingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Проверяет, не превысит ли добавление/обновление проводки лимит в 24 часа за день
        /// </summary>
        public async Task<bool> ValidateDailyHoursLimitAsync(int taskId, DateTime date, decimal hours, int? excludeEntryId = null)
        {
            // Получаем все проводки за указанную дату, исключая текущую (при обновлении)
            var query = _context.TimeEntries
                .Where(te => te.Date.Date == date.Date);

            if (excludeEntryId.HasValue)
            {
                query = query.Where(te => te.Id != excludeEntryId.Value);
            }

            var existingTotal = await query.SumAsync(te => te.Hours);

            // Проверяем, не превысит ли общая сумма 24 часа
            return existingTotal + hours <= 24;
        }

        /// <summary>
        /// Проверяет, можно ли редактировать задачу в проводке
        /// </summary>
        public async Task<bool> CanEditTaskInTimeEntryAsync(int timeEntryId)
        {
            var timeEntry = await _context.TimeEntries
                .Include(te => te.Task)
                .FirstOrDefaultAsync(te => te.Id == timeEntryId);

            if (timeEntry == null || timeEntry.Task == null)
                return false;

            // Задачу можно редактировать только если она активна
            return timeEntry.Task.IsActive;
        }

        /// <summary>
        /// Получить отчет за конкретный день с визуализацией
        /// </summary>
        public async Task<TimeEntryReportDto> GetReportForDayAsync(DateTime date)
        {
            var entries = await _context.TimeEntries
                .Include(te => te.Task)
                    .ThenInclude(t => t!.Project)
                .Where(te => te.Date.Date == date.Date)
                .OrderBy(te => te.Date)
                .ToListAsync();

            var totalHours = entries.Sum(e => e.Hours);
            var status = totalHours < 8 ? "under" : (totalHours == 8 ? "normal" : "over");
            var stickerColor = totalHours < 8 ? "yellow" : (totalHours == 8 ? "green" : "red");

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
        /// Получить отчет за неделю (день недели из выбранной даты)
        /// </summary>
        public async Task<TimeEntryReportDto> GetReportForWeekAsync(DateTime date)
        {
            var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);

            var entries = await _context.TimeEntries
                .Include(te => te.Task)
                    .ThenInclude(t => t!.Project)
                .Where(te => te.Date.Date >= startOfWeek && te.Date.Date < endOfWeek)
                .OrderBy(te => te.Date)
                .ToListAsync();

            var totalHours = entries.Sum(e => e.Hours);
            var avgDailyHours = totalHours / 7;
            var status = avgDailyHours < 8 ? "under" : (avgDailyHours == 8 ? "normal" : "over");
            var stickerColor = avgDailyHours < 8 ? "yellow" : (avgDailyHours == 8 ? "green" : "red");

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
        /// Получить отчет за месяц
        /// </summary>
        public async Task<TimeEntryReportDto> GetReportForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var entries = await _context.TimeEntries
                .Include(te => te.Task)
                    .ThenInclude(t => t!.Project)
                .Where(te => te.Date.Date >= startDate && te.Date.Date < endDate)
                .OrderBy(te => te.Date)
                .ToListAsync();

            var totalHours = entries.Sum(e => e.Hours);
            var workingDays = DateTime.DaysInMonth(year, month);
            var avgDailyHours = totalHours / workingDays;
            var status = avgDailyHours < 8 ? "under" : (avgDailyHours == 8 ? "normal" : "over");
            var stickerColor = avgDailyHours < 8 ? "yellow" : (avgDailyHours == 8 ? "green" : "red");

            return new TimeEntryReportDto
            {
                Date = startDate,
                TotalHours = totalHours,
                Status = status,
                StickerColor = stickerColor,
                Entries = entries.Select(MapToResponseDto).ToList()
            };
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
