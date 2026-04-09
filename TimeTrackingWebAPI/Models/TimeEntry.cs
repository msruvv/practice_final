namespace TimeTrackingWebAPI.Models
{
    /// <summary>
    /// Проводка времени
    /// </summary>
    public class TimeEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public string Description { get; set; } = string.Empty;
        public int TaskId { get; set; }
    }
}