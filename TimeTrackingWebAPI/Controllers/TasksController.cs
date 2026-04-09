using Microsoft.AspNetCore.Mvc;
using TimeTrackingWebAPI.DTO;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : Controller
    {
        private readonly ITimeTrackingRepository _repository;

        public TasksController(ITimeTrackingRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<TaskResponseDto> GetTasks([FromQuery] int? projectId = null, [FromQuery] bool includeInactive = false)
        {
            var tasks = _repository.GetTasks(projectId, includeInactive);

            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                ProjectName = _repository.GetProjectById(t.ProjectId)?.Name ?? string.Empty,
                IsActive = t.IsActive,
                //TotalHoursSpent = _repository.GetTimeEntries(null, null, t.Id).Sum(te => te.Hours)
            });
        }
    }
}
