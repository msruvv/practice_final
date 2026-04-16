using TimeTrackingWebAPI.Models.Enums;

namespace TimeTrackingWebAPI.DTO
{
    /// <summary>
    /// Отчет за день со стикером
    /// </summary>
    public class TimeEntryReportDto
    {
        /// <summary>
        /// Дата отчета
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Общее количество часов за день
        /// </summary>
        public decimal TotalHours { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public ReportStatus Status { get; set; }

        /// <summary>
        /// Цвет стикера
        /// </summary>
        public StickerColor StickerColor { get; set; }

        /// <summary>
        /// Сообщение для пользователя
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Список проводок за день
        /// </summary>
        public List<TimeEntryResponseDto> Entries { get; set; } = new();
    }
}