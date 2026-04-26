namespace TimeTrackingWebAPI.Models
{
    /// <summary>
    /// Проект компании.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Идентификатор проекта.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название проекта.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Уникальный код проекта.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Активность проекта.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Список задач проекта.
        /// </summary>
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}