using System.ComponentModel.DataAnnotations;

namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// DTO для создания и обновления проекта.
    /// </summary>
    public class ProjectRequestDto
    {
        /// <summary>
        /// Название проекта.
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Уникальный код проекта.
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Статус активности проекта.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}