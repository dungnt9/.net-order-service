namespace Application.Common.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(NotificationEvent notificationEvent);
}

public class NotificationEvent
{
    public string EventName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}