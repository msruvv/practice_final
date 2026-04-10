using System.ComponentModel.DataAnnotations;

namespace TimeTrackingWebAPI.DTO
{
    public class TimeEntryRequestDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, 24)]
        public decimal Hours { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int TaskId { get; set; }
    }
}
