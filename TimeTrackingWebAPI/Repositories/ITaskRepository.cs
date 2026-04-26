namespace TimeTrackingWebAPI.Repositories
{
    /// <summary>
    /// Репозиторий для работы с задачами.
    /// </summary>
    public interface ITaskRepository
    {
        /// <summary>
        /// Возвращает список задач.
        /// </summary>
        IEnumerable<Models.Task> GetTasks(int? projectId = null, bool includeInactive = false);

        /// <summary>
        /// Возвращает задачу по ID.
        /// </summary>
        Models.Task? GetTaskById(int id);

        /// <summary>
        /// Создает новую задачу.
        /// </summary>
        void CreateTask(Models.Task task);

        /// <summary>
        /// Обновляет задачу.
        /// </summary>
        void UpdateTask(Models.Task task);

        /// <summary>
        /// Удаляет задачу.
        /// </summary>
        Models.Task? DeleteTask(int id);

        /// <summary>
        /// Проверяет, можно ли редактировать задачу в проводке.
        /// </summary>
        bool CanEditTaskInTimeEntry(int timeEntryId);
    }
}
