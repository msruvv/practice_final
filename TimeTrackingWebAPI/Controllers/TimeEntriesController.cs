using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Controllers
{
    /// <summary>
    /// Управление проводками времени
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TimeEntriesController : ControllerBase
    {
        private readonly ITimeTrackingRepository _repository;

        public TimeEntriesController(ITimeTrackingRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Показывает список всех проводок
        /// </summary>
        /// <param name="fromDate">Начальная дата</param>
        /// <param name="toDate">Конечная дата</param>
        /// <param name="taskId">ID задачи</param>
        /// <returns>Список проводок</returns>
        [HttpGet]
        public IEnumerable<TimeEntryResponseDto> GetTimeEntries(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? taskId = null)
        {
            var entries = _repository.GetTimeEntries(fromDate, toDate, taskId);

            return entries.Select(e => new TimeEntryResponseDto
            {
                Id = e.Id,
                Date = e.Date,
                Hours = e.Hours,
                Description = e.Description,
                TaskId = e.TaskId,
                TaskName = _repository.GetTaskById(e.TaskId)?.Name ?? string.Empty,
                ProjectName = _repository.GetProjectById(_repository.GetTaskById(e.TaskId)?.ProjectId ?? 0)?.Name ?? string.Empty,
                CanEditTask = _repository.GetTaskById(e.TaskId)?.IsActive ?? false
            });
        }

        /// <summary>
        /// Показывает проводку по ID
        /// </summary>
        /// <param name="id">ID проводки</param>
        /// <returns>Данные проводки</returns>
        [HttpGet("{id}", Name = "GetTimeEntry")]
        public IActionResult GetTimeEntry(int id)
        {
            var entry = _repository.GetTimeEntryById(id);

            if (entry == null)
            {
                return NotFound();
            }

            return Ok(new TimeEntryResponseDto
            {
                Id = entry.Id,
                Date = entry.Date,
                Hours = entry.Hours,
                Description = entry.Description,
                TaskId = entry.TaskId,
                TaskName = _repository.GetTaskById(entry.TaskId)?.Name ?? string.Empty,
                ProjectName = _repository.GetProjectById(_repository.GetTaskById(entry.TaskId)?.ProjectId ?? 0)?.Name ?? string.Empty,
                CanEditTask = _repository.GetTaskById(entry.TaskId)?.IsActive ?? false
            });
        }

        /// <summary>
        /// Создает новую проводку
        /// </summary>
        /// <param name="entryDto">Данные проводки</param>
        /// <returns>Созданная проводка</returns>
        [HttpPost]
        public IActionResult CreateTimeEntry([FromBody] TimeEntryRequestDto entryDto)
        {
            if (entryDto == null)
            {
                return BadRequest();
            }

            // Проверка существования и активности задачи
            var task = _repository.GetTaskById(entryDto.TaskId);
            if (task == null)
            {
                return BadRequest($"Задача с ID {entryDto.TaskId} не найдена");
            }

            if (!task.IsActive)
            {
                return BadRequest("Нельзя создать проводку для неактивной задачи");
            }

            // Проверка лимита часов за день
            var dailyHours = _repository.GetDailyHoursSum(entryDto.Date, null);
            if (dailyHours + entryDto.Hours > 24)
            {
                return BadRequest("Суммарное количество часов за день не может превышать 24");
            }

            var timeEntry = new TimeEntry
            {
                Date = entryDto.Date.Date,
                Hours = entryDto.Hours,
                Description = entryDto.Description,
                TaskId = entryDto.TaskId
            };

            _repository.CreateTimeEntry(timeEntry);

            return CreatedAtRoute("GetTimeEntry", new { id = timeEntry.Id }, timeEntry);
        }

        /// <summary>
        /// Обновляет проводку
        /// </summary>
        /// <param name="id">ID проводки</param>
        /// <param name="entryDto">Новые данные проводки</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateTimeEntry(int id, [FromBody] TimeEntryUpdateDto entryDto)
        {
            if (entryDto == null)
            {
                return BadRequest();
            }

            var existingEntry = _repository.GetTimeEntryById(id);
            if (existingEntry == null)
            {
                return NotFound();
            }

            // Проверка: можно ли редактировать (задача должна быть активна)
            if (!_repository.CanEditTaskInTimeEntry(id))
            {
                return BadRequest("Нельзя редактировать проводку, так как задача стала неактивной");
            }

            // Проверка лимита часов за день (исключая текущую проводку)
            var dailyHours = _repository.GetDailyHoursSum(existingEntry.Date, id);
            if (dailyHours + entryDto.Hours > 24)
            {
                return BadRequest("Суммарное количество часов за день не может превышать 24");
            }

            existingEntry.Hours = entryDto.Hours;
            existingEntry.Description = entryDto.Description;

            _repository.UpdateTimeEntry(existingEntry);

            return NoContent();
        }

        /// <summary>
        /// Удаляет проводку
        /// </summary>
        /// <param name="id">ID проводки</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteTimeEntry(int id)
        {
            var deletedEntry = _repository.DeleteTimeEntry(id);

            if (deletedEntry == null)
            {
                return NotFound();
            }

            return Ok(deletedEntry);
        }
    }
}