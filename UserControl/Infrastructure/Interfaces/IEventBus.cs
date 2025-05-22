namespace UserControl.Infrastructure.Interfaces;

public interface IEventBus
{
    void PublishUserDeactivated(int userId);
    void PublishUserActivated(int userId);
}