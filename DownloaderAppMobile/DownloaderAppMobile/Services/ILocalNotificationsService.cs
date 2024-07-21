namespace DownloaderAppMobile.Services
{
    public interface ILocalNotificationsService
    {
        void ShowNotification(string title, string message, byte[] largeIconBytes = null);
    }
}
