using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Controllers
{
    /// <summary>
    /// Управление задачами проекта
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITimeTrackingRepository _repository;

        public TasksController(ITimeTrackingRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Показывает список всех задач
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="includeInactive">Включать неактивные</param>
        /// <returns>Список задач</returns>
        [HttpGet]
        public IEnumerable<TaskResponseDto> GetTasks([FromQuery] int? projectId = null, [FromQuery] bool includeInactive = false)
        {
            var tasks = _repository.GetTasks(projectId, includeInactive);

            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                ProjectName = _repository
                    .GetProjectById(t.ProjectId)?.Name ?? string.Empty,
                IsActive = t.IsActive,
                TotalHoursSpent = _repository
                    .GetTimeEntries(null, null, t.Id)
                    .Sum(te => te.Hours)
            });
        }

        /// <summary>
        /// Показывает задачу по ID
        /// </summary>
        /// <param name="id">ID задачи</param>
        /// <returns>Данные задачи</returns>
        [HttpGet("{id}", Name = "GetTask")]
        public IActionResult GetTask(int id)
        {
            var task = _repository.GetTaskById(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(new TaskResponseDto
            {
                Id = task.Id,
                Name = task.Name,
                ProjectId = task.ProjectId,
                ProjectName = _repository
                    .GetProjectById(task.ProjectId)?.Name ?? string.Empty,
                IsActive = task.IsActive,
                TotalHoursSpent = _repository
                    .GetTimeEntries(null, null, task.Id)
                    .Sum(te => te.Hours)
            });
        }

        /// <summary>
        /// Создает новую задачу
        /// </summary>
        /// <param name="taskDto">Данные задачи</param>
        /// <returns>Созданная задача</returns>
        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskRequestDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest();
            }

            // Проверка существования проекта
            var project = _repository.GetProjectById(taskDto.ProjectId);
            if (project == null)
            {
                return BadRequest($"Проект с ID {taskDto.ProjectId} не найден");
            }

            // Проверка уникальности названия задачи в проекте
            var existingTasks = _repository.GetTasks(taskDto.ProjectId, true);
            if (existingTasks.Any(t => t.Name == taskDto.Name))
            {
                return BadRequest($"Задача с названием '{taskDto.Name}' уже существует в этом проекте");
            }

            var task = new Models.Task
            {
                Name = taskDto.Name,
                ProjectId = taskDto.ProjectId,
                IsActive = taskDto.IsActive
            };

            _repository.CreateTask(task);

            return CreatedAtRoute("GetTask", new { id = task.Id }, task);
        }

        /// <summary>
        /// Обновляет задачу
        /// </summary>
        /// <param name="id">ID задачи</param>
        /// <param name="taskDto">Новые данные задачи</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskRequestDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest();
            }

            var existingTask = _repository.GetTaskById(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            // Проверка существования проекта
            var project = _repository.GetProjectById(taskDto.ProjectId);
            if (project == null)
            {
                return BadRequest($"Проект с ID {taskDto.ProjectId} не найден");
            }

            // Проверка уникальности названия
            var allTasks = _repository.GetTasks(taskDto.ProjectId, true);
            if (allTasks.Any(t => t.Name == taskDto.Name && t.Id != id))
            {
                return BadRequest($"Задача с названием '{taskDto.Name}' уже существует в этом проекте");
            }

            existingTask.Name = taskDto.Name;
            existingTask.ProjectId = taskDto.ProjectId;
            existingTask.IsActive = taskDto.IsActive;

            _repository.UpdateTask(existingTask);

            return NoContent();
        }

        /// <summary>
        /// Удаляет задачу
        /// </summary>
        /// <param name="id">ID задачи</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var deletedTask = _repository.DeleteTask(id);

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
