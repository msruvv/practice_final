using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;

namespace TimeTrackingWebAPI.Controllers
{
    /// <summary>
    /// Управление отчетами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ITimeTrackingRepository _repository;

        public ReportsController(ITimeTrackingRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Показывает отчет за день с дневной нормой
        /// </summary>
        /// <param name="date">Дата отчета</param>
        /// <returns>Отчет со стикером</returns>
        [HttpGet("day")]
        public IActionResult GetDayReport([FromQuery] DateTime date)
        {
            // Проверка на дневную норму
            var entries = _repository.GetTimeEntries(date, date.AddDays(1), null);
            var totalHours = entries.Sum(e => e.Hours);

            string status, stickerColor, message;

            if (totalHours < 8)
            {
                status = "under";
                stickerColor = "yellow";
                message = $"Внесено недостаточно часов";
            }
            else if (totalHours == 8)
            {
                status = "normal";
                stickerColor = "green";
                message = $"Норма часов выполнена!";
            }
            else
            {
                status = "over";
                stickerColor = "red";
                message = $"Переработка!";
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
                    TaskName = _repository
                        .GetTaskById(e.TaskId)?
                            .Name ?? string.Empty,
                    ProjectName = _repository
                        .GetProjectById(_repository
                            .GetTaskById(e.TaskId)?
                                .ProjectId ?? 0)?
                                    .Name ?? string.Empty,
                    CanEditTask = _repository
                        .GetTaskById(e.TaskId)?
                            .IsActive ?? false
                }).ToList()
            };

            return Ok(report);
        }

        /// <summary>
        /// Возвращает список проводок за неделю
        /// </summary>
        /// <param name="date">Дата в неделе</param>
        /// <returns>Список проводок</returns>
        [HttpGet("week")]
        public IActionResult GetWeekReport([FromQuery] DateTime date)
        {
            var startOfWeek = date.Date
                .AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);

            var entries = _repository.GetTimeEntries(startOfWeek, endOfWeek, null);

            var result = entries.Select(e => new TimeEntryResponseDto
            {
                Id = e.Id,
                Date = e.Date,
                Hours = e.Hours,
                Description = e.Description,
                TaskId = e.TaskId,
                TaskName = _repository
                    .GetTaskById(e.TaskId)?
                        .Name ?? string.Empty,
                ProjectName = _repository
                    .GetProjectById(_repository
                        .GetTaskById(e.TaskId)?
                            .ProjectId ?? 0)?
                                .Name ?? string.Empty,
                CanEditTask = _repository
                    .GetTaskById(e.TaskId)?
                        .IsActive ?? false
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Возвращает список проводок за месяц
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <returns>Список проводок</returns>
        [HttpGet("month")]
        public IActionResult GetMonthReport([FromQuery] int year, [FromQuery] int month)
        {
            if (year < 2000 || year > 2100 || month < 1 || month > 12)
            {
                return BadRequest("Некорректные параметры года или месяца");
            }

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var entries = _repository.GetTimeEntries(startDate, endDate, null);

            var result = entries.Select(e => new TimeEntryResponseDto
            {
                Id = e.Id,
                Date = e.Date,
                Hours = e.Hours,
                Description = e.Description,
                TaskId = e.TaskId,
                TaskName = _repository
                        .GetTaskById(e.TaskId)?
                            .Name ?? string.Empty,
                ProjectName = _repository
                        .GetProjectById(_repository
                            .GetTaskById(e.TaskId)?
                                .ProjectId ?? 0)?
                                    .Name ?? string.Empty,
                CanEditTask = _repository
                        .GetTaskById(e.TaskId)?
                            .IsActive ?? false
            }).ToList();

            return Ok(result);
        }
    }
}