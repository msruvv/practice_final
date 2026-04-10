namespace TimeTrackingWebAPI.DTO
{
    public class TimeEntryReportDto
    {
        public DateTime Date { get; set; }
        public decimal TotalHours { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StickerColor { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<TimeEntryResponseDto> Entries { get; set; } = new();
    }
}
