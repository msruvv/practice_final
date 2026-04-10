using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;

namespace TimeTrackingWebAPI.Controllers
{
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
        /// Отчет за день с дневной нормой
        /// </summary>
        [HttpGet("day")]
        public IActionResult GetDayReport([FromQuery] DateTime date)
        {
            var entries = _repository.GetTimeEntries(date, date.AddDays(1), null);
            var totalHours = entries.Sum(e => e.Hours);

            string status, stickerColor, message;

            if (totalHours < 8)
            {
                status = "under";
                stickerColor = "yellow";
                message = $"Внесено недостаточно часов (необходимо 8, внесено {totalHours})";
            }
            else if (totalHours == 8)
            {
                status = "normal";
                stickerColor = "green";
                message = $"Норма часов выполнена! (8 часов)";
            }
            else
            {
                status = "over";
                stickerColor = "red";
                message = $"Переработка! (внесено {totalHours} часов)";
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
        /// Отчет за неделю - список проводок
        /// </summary>
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
        /// Отчет за месяц - список проводок
        /// </summary>
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