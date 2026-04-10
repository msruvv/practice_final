using System.ComponentModel.DataAnnotations;

namespace TimeTrackingWebAPI.DTO
{
    public class TaskRequestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
