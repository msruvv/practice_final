using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;
using TimeTrackingWebAPI.Repositories;

namespace TimeTrackingWebAPI.Controllers
{
    /// <summary>
    /// Управление проектами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        /// <summary>
        /// Репозиторий для работы с задачами
        /// </summary>
        private readonly ITaskRepository _taskRepository;

        /// <summary>
        /// Репозиторий для работы с проектами
        /// </summary>
        private readonly IProjectRepository _projectRepository;

        /// <summary>
        /// Конструктор контроллера проектов
        /// </summary>
        /// <param name="taskRepository">Репозиторий задач</param>
        /// <param name="projectRepository">Репозиторий проектов</param>
        public ProjectsController(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Возвращает список всех проектов
        /// </summary>
        /// <param name="includeInactive">Включать неактивные</param>
        /// <returns>Список проектов</returns>
        [HttpGet]
        public IEnumerable<ProjectResponseDto> GetProjects(
            [FromQuery] bool includeInactive = false)
        {
            var projects = _projectRepository.GetProjects(includeInactive);

            return projects.Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                IsActive = p.IsActive,
                TasksCount = _taskRepository.GetTasks(p.Id, true).Count()
            });
        }

        /// <summary>
        /// Получает проект по ID
        /// </summary>
        /// <param name="id">ID проекта</param>
        /// <returns>Данные проекта</returns>
        [HttpGet("{id}", Name = "GetProject")]
        public IActionResult GetProject(int id)
        {
            var project = _projectRepository.GetProjectById(id);

            if (project == null)
                return NotFound();

            return Ok(new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Code = project.Code,
                IsActive = project.IsActive,
                TasksCount = _taskRepository.GetTasks(project.Id, true).Count()
            });
        }

        /// <summary>
        /// Создает новый проект
        /// </summary>
        /// <param name="projectDto">Данные проекта</param>
        /// <returns>Созданный проект</returns>
        [HttpPost]
        public IActionResult CreateProject(
            [FromBody] ProjectRequestDto projectDto)
        {
            if (projectDto == null)
                return BadRequest();

            // Проверка уникальности кода
            var existingProjects = _projectRepository.GetProjects(true);
            if (existingProjects.Any(p => p.Code == projectDto.Code))
                return BadRequest(
                    $"Проект с кодом '{projectDto.Code}' уже существует");

            var project = new Project
            {
                Name = projectDto.Name,
                Code = projectDto.Code,
                IsActive = projectDto.IsActive
            };

            _projectRepository.CreateProject(project);

            return CreatedAtRoute("GetProject",
                new { id = project.Id }, project);
        }

        /// <summary>
        /// Обновляет проект
        /// </summary>
        /// <param name="id">ID проекта</param>
        /// <param name="projectDto">Новые данные проекта</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateProject(int id, 
            [FromBody] ProjectRequestDto projectDto)
        {
            if (projectDto == null)
                return BadRequest();

            var existingProject = _projectRepository.GetProjectById(id);
            if (existingProject == null)
                return NotFound();

            // Проверка уникальности кода
            var allProjects = _projectRepository.GetProjects(true);
            if (allProjects.Any(p => p.Code == projectDto.Code && p.Id != id))
                return BadRequest(
                    $"Проект с кодом '{projectDto.Code}' уже существует");

            existingProject.Name = projectDto.Name;
            existingProject.Code = projectDto.Code;
            existingProject.IsActive = projectDto.IsActive;

            _projectRepository.UpdateProject(existingProject);

            return NoContent();
        }

        /// <summary>
        /// Удаляет проект
        /// </summary>
        /// <param name="id">ID проекта</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            var tasks = _taskRepository.GetTasks(id, true);
            if (tasks.Any(t => t.IsActive))
                return BadRequest(
                    "Нельзя удалить проект с активными задачами");

            var deletedProject = _projectRepository.DeleteProject(id);

            if (deletedProject == null)
                return NotFound();

            return Ok(deletedProject);
        }
    }
}