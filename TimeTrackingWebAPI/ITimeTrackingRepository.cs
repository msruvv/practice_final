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
        Models.Task? GetTaskById(int id);
        void CreateTask(Models.Task task);
        void UpdateTask(Models.Task task);
        Models.Task? DeleteTask(int id);

        IEnumerable<TimeEntry> GetTimeEntries(DateTime? fromDate = null, DateTime? toDate = null, int? taskId = null);
        TimeEntry? GetTimeEntryById(int id);
        void CreateTimeEntry(TimeEntry timeEntry);
        void UpdateTimeEntry(TimeEntry timeEntry);
        TimeEntry? DeleteTimeEntry(int id);

        decimal GetDailyHoursSum(DateTime date, int? excludeEntryId = null);
        bool CanEditTaskInTimeEntry(int timeEntryId);
    }
}
