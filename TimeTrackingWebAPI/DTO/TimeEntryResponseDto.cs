namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Ответ по проводке.
    /// </summary>
    public class TimeEntryResponseDto
    {
        /// <summary>
        /// Идентификатор проводки.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата проводки.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Количество часов.
        /// </summary>
        public decimal Hours { get; set; }

        /// <summary>
        /// Описание работы.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор задачи.
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Название задачи.
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Название проекта.
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Можно ли редактировать задачу.
        /// </summary>
        public bool CanEditTask { get; set; }
    }
}