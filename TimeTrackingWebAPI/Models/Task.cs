namespace TimeTrackingWebAPI.Models
{
    /// <summary>
    /// Задача проекта
    /// </summary>
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
