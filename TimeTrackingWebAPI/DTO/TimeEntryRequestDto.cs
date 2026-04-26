using System.ComponentModel.DataAnnotations;

namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Запрос на создание проводки.
    /// </summary>
    public class TimeEntryRequestDto
    {
        /// <summary>
        /// Дата проводки.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Количество часов.
        /// </summary>
        [Required]
        [Range(0.01, 24)]
        public decimal Hours { get; set; }

        /// <summary>
        /// Описание работы.
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор задачи.
        /// </summary>
        [Required]
        public int TaskId { get; set; }
    }
}