using System.ComponentModel.DataAnnotations;

namespace TimeTrackingWebAPI.DTO
{
    public class ProjectRequestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Code { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
