namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Ответ по задаче.
    /// </summary>
    public class TaskResponseDto
    {
        /// <summary>
        /// Идентификатор задачи.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название задачи.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор проекта.
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Название проекта.
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Статус активности задачи.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Общее количество часов, потраченных на задачу.
        /// </summary>
        public decimal TotalHoursSpent { get; set; }
    }
}