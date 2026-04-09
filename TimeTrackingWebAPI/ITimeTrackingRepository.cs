using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI
{
    /// <summary>
    /// Интерфейс репозитория для работы с данными
    /// </summary>
    public interface ITimeTrackingRepository
    {
        IEnumerable<Project> GetProjects(bool includeInactive = false);
        Project? GetProjectById(int id);
        void CreateProject(Project project);
        void UpdateProject(Project project);
        Project? DeleteProject(int id);

        IEnumerable<Models.Task> GetTasks(int? projectId = null, bool includeInactive = false);
    }
}
