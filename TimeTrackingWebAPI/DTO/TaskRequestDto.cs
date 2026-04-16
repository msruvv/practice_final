using System.ComponentModel.DataAnnotations;

namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Запрос на создание/обновление задачи
    /// </summary>
    public class TaskRequestDto
    {
        /// <summary>
        /// Название задачи
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        [Required]
        public int ProjectId { get; set; }

        /// <summary>
        /// Статус активности задачи
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}