namespace TimeTrackingWebAPI.DTO
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        //public decimal TotalHoursSpent { get; set; }
    }
}
