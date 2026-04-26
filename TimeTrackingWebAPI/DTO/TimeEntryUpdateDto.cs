using System.ComponentModel.DataAnnotations;

namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Запрос на обновление проводки.
    /// </summary>
    public class TimeEntryUpdateDto
    {
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
    }
}