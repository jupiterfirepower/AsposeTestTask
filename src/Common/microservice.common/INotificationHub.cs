namespace microservice.common;

public interface INotificationHub
{
    Task PushDataAsync(Notification data, CancellationToken cancellationToken);
}
