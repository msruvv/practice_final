using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI
{
    public class EFTimeTrackingRepository : ITimeTrackingRepository
    {
        private readonly TimeTrackingDbContext _context;

        public EFTimeTrackingRepository(TimeTrackingDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Project> GetProjects(bool includeInactive = false)
        {
            var query = _context.Projects.AsQueryable();
            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            return query.ToList();
        }

        public Project? GetProjectById(int id)
        {
            return _context.Projects.Find(id);
        }

        public void CreateProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        public void UpdateProject(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }

        public Project? DeleteProject(int id)
        {
            var project = GetProjectById(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            return project;
        }

        public IEnumerable<Models.Task> GetTasks(
            int? projectId = null,
            bool includeInactive = false)
        {
            var query = _context.Tasks.AsQueryable();

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            if (!includeInactive)
                query = query.Where(t => t.IsActive);

            return query.ToList();
        }

        public Models.Task? GetTaskById(int id)
        {
            return _context.Tasks.Find(id);
        }

        public void CreateTask(Models.Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void UpdateTask(Models.Task task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }

        public Models.Task? DeleteTask(int id)
        {
            var task = GetTaskById(id);
            if (task != null)
            {
                // Проверка: нет ли проводок у задачи
                var hasEntries = _context.TimeEntries.Any(te => te.TaskId == id);
                if (hasEntries)
                    throw new InvalidOperationException(
                        "Нельзя удалить задачу, по которой есть списанные часы");

                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
            return task;
        }

        public IEnumerable<TimeEntry> GetTimeEntries(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? taskId = null)
        {
            var query = _context.TimeEntries.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(te => te.Date.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(te => te.Date.Date <= toDate.Value.Date);
            }

            if (taskId.HasValue)
            {
                query = query.Where(te => te.TaskId == taskId.Value);
            }

            return query.OrderByDescending(te => te.Date).ToList();
        }

        public TimeEntry? GetTimeEntryById(int id)
        {
            return _context.TimeEntries.Find(id);
        }

        public void CreateTimeEntry(TimeEntry timeEntry)
        {
            _context.TimeEntries.Add(timeEntry);
            _context.SaveChanges();
        }

        public void UpdateTimeEntry(TimeEntry timeEntry)
        {
            _context.TimeEntries.Update(timeEntry);
            _context.SaveChanges();
        }

        public TimeEntry? DeleteTimeEntry(int id)
        {
            var timeEntry = GetTimeEntryById(id);
            if (timeEntry != null)
            {
                _context.TimeEntries.Remove(timeEntry);
                _context.SaveChanges();
            }
            return timeEntry;
        }

        public decimal GetDailyHoursSum(DateTime date, int? excludeEntryId = null)
        {
            var query = _context.TimeEntries
                .Where(te => te.Date.Date == date.Date);

            if (excludeEntryId.HasValue)
            {
                query = query
                    .Where(te => te.Id != excludeEntryId.Value);
            }

            return query.Sum(te => te.Hours);
        }

        public bool CanEditTaskInTimeEntry(int timeEntryId)
        {
            var isTaskActive = _context.TimeEntries
                .Where(te => te.Id == timeEntryId)
                .Join(_context.Tasks,
                    te => te.TaskId,
                    t => t.Id,
                    (te, t) => t.IsActive)
                .FirstOrDefault();

            return isTaskActive;
        }
    }
}
