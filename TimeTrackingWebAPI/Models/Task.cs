namespace TimeTrackingWebAPI.Models
{
    /// <summary>
    /// Задача проекта
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название задачи
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Активность задачи
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Проект задачи
        /// </summary>
        public virtual Project? Project { get; set; }

        /// <summary>
        /// Список проводок по задаче
        /// </summary>
        public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    }
}