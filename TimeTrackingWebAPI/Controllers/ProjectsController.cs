using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ITimeTrackingRepository _repository;

        public ProjectsController(ITimeTrackingRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<ProjectResponseDto> GetProjects([FromQuery] bool includeInactive = false)
        {
            var projects = _repository.GetProjects(includeInactive);

            return projects.Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                IsActive = p.IsActive,
                TasksCount = 3
            });
        }

        [HttpGet("{id}", Name = "GetProject")]
        public IActionResult GetProject(int id)
        {
            var project = _repository.GetProjectById(id);

            if (project == null)
                return NotFound();

            return Ok(new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Code = project.Code,
                IsActive = project.IsActive,
                TasksCount = 3
            });
        }

        [HttpPost]
        public IActionResult CreateProject([FromBody] ProjectRequestDto projectDto)
        {
            if (projectDto == null)
                return BadRequest();

            // Проверка уникальности кода
            var existingProjects = _repository.GetProjects(true);
            if (existingProjects.Any(p => p.Code == projectDto.Code))
                return BadRequest($"Проект с кодом '{projectDto.Code}' уже существует");

            var project = new Project
            {
                Name = projectDto.Name,
                Code = projectDto.Code,
                IsActive = projectDto.IsActive
            };

            _repository.CreateProject(project);

            return CreatedAtRoute("GetProject", new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProject(int id, [FromBody] ProjectRequestDto projectDto)
        {
            if (projectDto == null)
                return BadRequest();

            var existingProject = _repository.GetProjectById(id);
            if (existingProject == null)
                return NotFound();

            // Проверка уникальности кода
            var allProjects = _repository.GetProjects(true);
            if (allProjects.Any(p => p.Code == projectDto.Code && p.Id != id))
                return BadRequest($"Проект с кодом '{projectDto.Code}' уже существует");

            existingProject.Name = projectDto.Name;
            existingProject.Code = projectDto.Code;
            existingProject.IsActive = projectDto.IsActive;

            _repository.UpdateProject(existingProject);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            // Проверка: есть ли активные задачи у проекта
            //var tasks = _repository.GetTasks(id, true);
            //if (tasks.Any(t => t.IsActive))
            //    return BadRequest("Нельзя удалить проект с активными задачами");

            var deletedProject = _repository.DeleteProject(id);

            if (deletedProject == null)
                return NotFound();

            return Ok(deletedProject);
        }
    }
}
