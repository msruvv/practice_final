namespace TimeTrackingWebAPI.DTO
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TasksCount { get; set; }
    }
}
