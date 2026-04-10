namespace TimeTrackingWebAPI.Models
{
    /// <summary>
    /// Проводка рабочего времени
    /// </summary>
    public class TimeEntry
    {
        /// <summary>
        /// Идентификатор проводки
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата проводки
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Количество часов
        /// </summary>
        public decimal Hours { get; set; }

        /// <summary>
        /// Описание работы
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Задача проводки
        /// </summary>
        public virtual Task? Task { get; set; }
    }
}