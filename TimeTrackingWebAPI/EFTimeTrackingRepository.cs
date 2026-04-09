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

        public IEnumerable<Models.Task> GetTasks(int? projectId = null, bool includeInactive = false)
        {
            var query = _context.Tasks.AsQueryable();

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            if (!includeInactive)
                query = query.Where(t => t.IsActive);

            return query.ToList();
        }
    }
}
