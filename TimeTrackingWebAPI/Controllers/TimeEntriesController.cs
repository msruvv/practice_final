using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;
using TimeTrackingWebAPI.Repositories;

namespace TimeTrackingWebAPI.Controllers
{
    /// <summary>
    /// Управление проводками времени
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TimeEntriesController : ControllerBase
    {
        /// <summary>
        /// Количество часов в дне
        /// </summary>
        private const int HoursInDay = 24;

        /// <summary>
        /// Репозиторий для работы с проводками времени
        /// </summary>
        private readonly ITimeEntryRepository _timeEntryRepository;

        /// <summary>
        /// Репозиторий для работы с задачами
        /// </summary>
        private readonly ITaskRepository _taskRepository;

        /// <summary>
        /// Репозиторий для работы с проектами
        /// </summary>
        private readonly IProjectRepository _projectRepository;

        /// <summary>
        /// Конструктор контроллера проводок
        /// </summary>
        /// <param name="timeEntryRepository">Репозиторий проводок времени</param>
        /// <param name="taskRepository">Репозиторий задач</param>
        /// <param name="projectRepository">Репозиторий проектов</param>
        public TimeEntriesController(
            ITimeEntryRepository timeEntryRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
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
            var entries = _timeEntryRepository.GetTimeEntries(fromDate, toDate, taskId);

            return entries.Select(e => new TimeEntryResponseDto
            {
                Id = e.Id,
                Date = e.Date,
                Hours = e.Hours,
                Description = e.Description,
                TaskId = e.TaskId,
                TaskName = _taskRepository
                    .GetTaskById(e.TaskId)?.Name ?? string.Empty,
                ProjectName = _projectRepository
                    .GetProjectById(_taskRepository
                        .GetTaskById(e.TaskId)?.ProjectId ?? 0)?.Name ?? string.Empty,
                CanEditTask = _taskRepository.GetTaskById(e.TaskId)?.IsActive ?? false
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
            var entry = _timeEntryRepository.GetTimeEntryById(id);

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
                TaskName = _taskRepository
                    .GetTaskById(entry.TaskId)?.Name ?? string.Empty,
                ProjectName = _projectRepository
                    .GetProjectById(_taskRepository
                        .GetTaskById(entry.TaskId)?.ProjectId ?? 0)?.Name ?? string.Empty,
                CanEditTask = _taskRepository.GetTaskById(entry.TaskId)?.IsActive ?? false
            });
        }

        /// <summary>
        /// Создает новую проводку
        /// </summary>
        /// <param name="entryDto">Данные проводки</param>
        /// <returns>Созданная проводка</returns>
        [HttpPost]
        public IActionResult CreateTimeEntry(
            [FromBody] TimeEntryRequestDto entryDto)
        {
            if (entryDto == null)
            {
                return BadRequest();
            }

            // Проверка существования и активности задачи
            var task = _taskRepository.GetTaskById(entryDto.TaskId);
            if (task == null)
            {
                return BadRequest($"Задача с ID {entryDto.TaskId} не найдена");
            }

            if (!task.IsActive)
            {
                return BadRequest("Нельзя создать проводку для неактивной задачи");
            }

            // Проверка лимита часов за день
            var dailyHours = _timeEntryRepository.GetDailyHoursSum(entryDto.Date, null);
            if (dailyHours + entryDto.Hours > HoursInDay)
            {
                return BadRequest("Суммарное количество часов за день превышает лимит!");
            }

            var timeEntry = new TimeEntry
            {
                Date = entryDto.Date.Date,
                Hours = entryDto.Hours,
                Description = entryDto.Description,
                TaskId = entryDto.TaskId
            };

            _timeEntryRepository.CreateTimeEntry(timeEntry);

            return CreatedAtRoute("GetTimeEntry", new { id = timeEntry.Id }, timeEntry);
        }

        /// <summary>
        /// Обновляет проводку
        /// </summary>
        /// <param name="id">ID проводки</param>
        /// <param name="entryDto">Новые данные проводки</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateTimeEntry(int id,
            [FromBody] TimeEntryUpdateDto entryDto)
        {
            if (entryDto == null)
            {
                return BadRequest();
            }

            var existingEntry = _timeEntryRepository.GetTimeEntryById(id);
            if (existingEntry == null)
            {
                return NotFound();
            }

            // Проверка: можно ли редактировать (задача должна быть активна)
            if (!_taskRepository.CanEditTaskInTimeEntry(id))
            {
                return BadRequest(
                    "Нельзя редактировать проводку, так как задача стала неактивной");
            }

            // Проверка лимита часов за день (исключая текущую проводку)
            var dailyHours = _timeEntryRepository
                .GetDailyHoursSum(existingEntry.Date, id);
            if (dailyHours + entryDto.Hours > HoursInDay)
            {
                return BadRequest(
                    "Суммарное количество часов за день превышает лимит!");
            }

            existingEntry.Hours = entryDto.Hours;
            existingEntry.Description = entryDto.Description;

            _timeEntryRepository.UpdateTimeEntry(existingEntry);

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
            var deletedEntry = _timeEntryRepository.DeleteTimeEntry(id);

            if (deletedEntry == null)
            {
                return NotFound();
            }

            return Ok(deletedEntry);
        }
    }
}