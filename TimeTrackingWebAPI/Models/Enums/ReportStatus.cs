namespace TimeTrackingWebAPI.Models.Enums
{
    /// <summary>
    /// Статус отчета по часам.
    /// </summary>
    public enum ReportStatus
    {
        /// <summary>
        /// Недостаточно часов.
        /// </summary>
        Under,
        
        /// <summary>
        /// Достаточно часов.
        /// </summary>
        Normal,

        /// <summary>
        /// Переработка.
        /// </summary>
        Over
    }
}
