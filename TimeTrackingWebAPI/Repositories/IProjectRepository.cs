using TimeTrackingWebAPI.Models;

namespace TimeTrackingWebAPI.Repositories
{
    /// <summary>
    /// Репозиторий для работы с проектами
    /// </summary>
    public interface IProjectRepository
    {
        /// <summary>
        /// Возвращает список проектов
        /// </summary>
        IEnumerable<Project> GetProjects(bool includeInactive = false);

        /// <summary>
        /// Возвращает проект по ID
        /// </summary>
        Project? GetProjectById(int id);

        /// <summary>
        /// Создает новый проект
        /// </summary>
        void CreateProject(Project project);

        /// <summary>
        /// Обновляет проект
        /// </summary>
        void UpdateProject(Project project);

        /// <summary>
        /// Удаляет проект
        /// </summary>
        Project? DeleteProject(int id);
    }
}
