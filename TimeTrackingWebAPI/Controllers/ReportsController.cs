using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models.Enums;
using TimeTrackingWebAPI.Repositories;

namespace TimeTrackingWebAPI.Controllers
{
    /// <summary>
    /// Управление отчетами.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        /// <summary>
        /// Норма рабочих часов в день.
        /// </summary>
        private const int DailyNormHours = 8;

        /// <summary>
        /// Минимальный допустимый год.
        /// </summary>
        private const int MinYear = 2000;

        /// <summary>
        /// Максимальный допустимый год.
        /// </summary>
        private const int MaxYear = 2100;

        /// <summary>
        /// Минимальный номер месяца.
        /// </summary>
        private const int MinMonth = 1;

        /// <summary>
        /// Максимальный номер месяца.
        /// </summary>
        private const int MaxMonth = 12;

        /// <summary>
        /// Количество дней в неделе.
        /// </summary>
        private const int DaysInWeek = 7;

        /// <summary>
        /// Количество месяцев для добавления.
        /// </summary>
        private const int MonthsToAdd = 1;

        /// <summary>
        /// Репозиторий для работы с проводками.
        /// </summary>
        private readonly ITimeEntryRepository _timeEntryRepository;

        /// <summary>
        /// Репозиторий для работы с задачами.
        /// </summary>
        private readonly ITaskRepository _taskRepository;

        /// <summary>
        /// Репозиторий для работы с проектами.
        /// </summary>
        private readonly IProjectRepository _projectRepository;

        /// <summary>
        /// Конструктор контроллера отчетов.
        /// </summary>
        /// <param name="timeEntryRepository">Репозиторий проводок времени.</param>
        /// <param name="taskRepository">Репозиторий задач.</param>
        /// <param name="projectRepository">Репозиторий проектов.</param>
        public ReportsController(
            ITimeEntryRepository timeEntryRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Показывает отчет за день с дневной нормой.
        /// </summary>
        /// <param name="date">Дата отчета.</param>
        /// <returns>Отчет со стикером.</returns>
        [HttpGet("day")]
        public IActionResult GetDayReport([FromQuery] DateTime date)
        {
            var entries = _timeEntryRepository
                .GetTimeEntries(date, date.AddDays(1), null);
            var totalHours = entries.Sum(e => e.Hours);

            string message;
            ReportStatus status;
            StickerColor stickerColor;

            if (totalHours < DailyNormHours)
            {
                status = ReportStatus.Under;
                stickerColor = StickerColor.Yellow;
                message = "Внесено недостаточно часов";
            }
            else if (totalHours == DailyNormHours)
            {
                status = ReportStatus.Normal;
                stickerColor = StickerColor.Green;
                message = "Норма часов выполнена!";
            }
            else
            {
                status = ReportStatus.Over;
                stickerColor = StickerColor.Red;
                message = "Переработка!";
            }

            var report = new TimeEntryReportDto
            {
                Date = date,
                TotalHours = totalHours,
                Status = status,
                StickerColor = stickerColor,
                Message = message,
                Entries = entries.Select(e => new TimeEntryResponseDto
                {
                    Id = e.Id,
                    Date = e.Date,
                    Hours = e.Hours,
                    Description = e.Description,
                    TaskId = e.TaskId,
                    TaskName = _taskRepository
                        .GetTaskById(e.TaskId)?.Name ?? string.Empty,
                    ProjectName = _projectRepository.GetProjectById(
                        _taskRepository.GetTaskById(
                            e.TaskId)?.ProjectId ?? 0)?.Name ?? string.Empty,
                    CanEditTask = _taskRepository
                        .GetTaskById(e.TaskId)?.IsActive ?? false
                }).ToList()
            };

            return Ok(report);
        }

        /// <summary>
        /// Возвращает список проводок за неделю.
        /// </summary>
        /// <param name="date">Дата в неделе.</param>
        /// <returns>Список проводок.</returns>
        [HttpGet("week")]
        public IActionResult GetWeekReport([FromQuery] DateTime date)
        {
            var startOfWeek = date.Date
                .AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(DaysInWeek);

            var entries = _timeEntryRepository
                .GetTimeEntries(startOfWeek, endOfWeek, null);

            var result = entries.Select(e => new TimeEntryResponseDto
            {
                Id = e.Id,
                Date = e.Date,
                Hours = e.Hours,
                Description = e.Description,
                TaskId = e.TaskId,
                TaskName = _taskRepository
                    .GetTaskById(e.TaskId)?.Name ?? string.Empty,
                ProjectName = _projectRepository.GetProjectById(
                    _taskRepository.GetTaskById(
                        e.TaskId)?.ProjectId ?? 0)?.Name ?? string.Empty,
                CanEditTask = _taskRepository
                    .GetTaskById(e.TaskId)?.IsActive ?? false
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Возвращает список проводок за месяц.
        /// </summary>
        /// <param name="year">Год.</param>
        /// <param name="month">Месяц.</param>
        /// <returns>Список проводок.</returns>
        [HttpGet("month")]
        public IActionResult GetMonthReport([FromQuery] int year, [FromQuery] int month)
        {
            if (year < MinYear || year > MaxYear || month < MinMonth || month > MaxMonth)
            {
                return BadRequest("Некорректные параметры года или месяца");
            }

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(MonthsToAdd);

            var entries = _timeEntryRepository.GetTimeEntries(startDate, endDate, null);

            var result = entries.Select(e => new TimeEntryResponseDto
            {
                Id = e.Id,
                Date = e.Date,
                Hours = e.Hours,
                Description = e.Description,
                TaskId = e.TaskId,
                TaskName = _taskRepository
                    .GetTaskById(e.TaskId)?.Name ?? string.Empty,
                ProjectName = _projectRepository.GetProjectById(
                    _taskRepository.GetTaskById(
                        e.TaskId)?.ProjectId ?? 0)?.Name ?? string.Empty,
                CanEditTask = _taskRepository
                    .GetTaskById(e.TaskId)?.IsActive ?? false
            }).ToList();

            return Ok(result);
        }
    }
}