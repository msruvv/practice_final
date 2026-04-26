namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Ответ по проекту.
    /// </summary>
    public class ProjectResponseDto
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
        /// Код проекта.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Статус активности.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Количество задач в проекте.
        /// </summary>
        public int TasksCount { get; set; }
    }
}