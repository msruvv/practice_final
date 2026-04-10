namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Ответ по проводке
    /// </summary>
    public class TimeEntryResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public string Description { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public bool CanEditTask { get; set; }
    }
}
