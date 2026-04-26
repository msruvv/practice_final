using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;
using TimeTrackingWebAPI.Repositories;

namespace TimeTrackingWebAPI.Controllers
{
    /// <summary>
    /// Управление задачами проекта.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        /// <summary>
        /// Репозиторий для работы с проводками времени.
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
        /// Конструктор контроллера задач.
        /// </summary>
        /// <param name="timeEntryRepository">Репозиторий проводок времени.</param>
        /// <param name="taskRepository">Репозиторий задач.</param>
        /// <param name="projectRepository">Репозиторий проектов.</param>
        public TasksController(
            ITimeEntryRepository timeEntryRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Показывает список всех задач.
        /// </summary>
        /// <param name="projectId">ID проекта.</param>
        /// <param name="includeInactive">Включать неактивные.</param>
        /// <returns>Список задач.</returns>
        [HttpGet]
        public IEnumerable<TaskResponseDto> GetTasks(
            [FromQuery] int? projectId = null,
            [FromQuery] bool includeInactive = false)
        {
            var tasks = _taskRepository.GetTasks(projectId, includeInactive);

            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                ProjectName = _projectRepository
                    .GetProjectById(t.ProjectId)?.Name ?? string.Empty,
                IsActive = t.IsActive,
                TotalHoursSpent = _timeEntryRepository
                    .GetTimeEntries(null, null, t.Id)
                        .Sum(te => te.Hours)
            });
        }

        /// <summary>
        /// Показывает задачу по ID.
        /// </summary>
        /// <param name="id">ID задачи.</param>
        /// <returns>Данные задачи.</returns>
        [HttpGet("{id}", Name = "GetTask")]
        public IActionResult GetTask(int id)
        {
            var task = _taskRepository.GetTaskById(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(new TaskResponseDto
            {
                Id = task.Id,
                Name = task.Name,
                ProjectId = task.ProjectId,
                ProjectName = _projectRepository
                    .GetProjectById(task.ProjectId)?.Name ?? string.Empty,
                IsActive = task.IsActive,
                TotalHoursSpent = _timeEntryRepository
                    .GetTimeEntries(null, null, task.Id).Sum(te => te.Hours)
            });
        }

        /// <summary>
        /// Создает новую задачу.
        /// </summary>
        /// <param name="taskDto">Данные задачи.</param>
        /// <returns>Созданная задача.</returns>
        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskRequestDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest();
            }

            // Проверка существования проекта.
            var project = _projectRepository.GetProjectById(taskDto.ProjectId);
            if (project == null)
            {
                return BadRequest($"Проект с ID {taskDto.ProjectId} не найден");
            }

            // Проверка уникальности названия задачи в проекте.
            var existingTasks = _taskRepository.GetTasks(taskDto.ProjectId, true);
            if (existingTasks.Any(t => t.Name == taskDto.Name))
            {
                return BadRequest(
                    $"Задача с названием '{taskDto.Name}' уже существует в проекте");
            }

            var task = new Models.Task
            {
                Name = taskDto.Name,
                ProjectId = taskDto.ProjectId,
                IsActive = taskDto.IsActive
            };

            _taskRepository.CreateTask(task);

            return CreatedAtRoute("GetTask",new { id = task.Id }, task);
        }

        /// <summary>
        /// Обновляет задачу.
        /// </summary>
        /// <param name="id">ID задачи</param>
        /// <param name="taskDto">Новые данные задачи.</param>
        /// <returns>Результат обновления.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id,
            [FromBody] TaskRequestDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest();
            }

            var existingTask = _taskRepository.GetTaskById(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            // Проверка существования проекта.
            var project = _projectRepository.GetProjectById(taskDto.ProjectId);
            if (project == null)
            {
                return BadRequest($"Проект с ID {taskDto.ProjectId} не найден");
            }

            // Проверка уникальности названия.
            var allTasks = _taskRepository.GetTasks(taskDto.ProjectId, true);
            if (allTasks.Any(t => t.Name == taskDto.Name && t.Id != id))
            {
                return BadRequest(
                    $"Задача с названием '{taskDto.Name}' уже существует в этом проекте");
            }

            existingTask.Name = taskDto.Name;
            existingTask.ProjectId = taskDto.ProjectId;
            existingTask.IsActive = taskDto.IsActive;

            _taskRepository.UpdateTask(existingTask);

            return NoContent();
        }

        /// <summary>
        /// Удаляет задачу.
        /// </summary>
        /// <param name="id">ID задачи.</param>
        /// <returns>Результат удаления.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var deletedTask = _taskRepository.DeleteTask(id);

                if (deletedTask == null)
                {
                    return NotFound();
                }

                return Ok(deletedTask);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}