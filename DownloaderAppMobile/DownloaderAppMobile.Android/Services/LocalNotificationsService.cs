using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Xamarin.Essentials;
using DownloaderAppMobile.Droid;
using DownloaderAppMobile.Services;
using AndroidX.Core.Content;

[assembly: Xamarin.Forms.Dependency(typeof(LocalNotificationsService))]
namespace DownloaderAppMobile.Droid
{
    public class LocalNotificationsService : ILocalNotificationsService
    {
        private const string CHANNEL_ID = "local_notifications_channel";
        private const string CHANNEL_NAME = "Notifications";
        private const string CHANNEL_DESCRIPTION = "Local and push notifications messages appear in this channel";

        private int notificationId = -1;

        private bool isChannelInitialized;

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.High)
            {
                Description = CHANNEL_DESCRIPTION,
            };

            var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        public void ShowNotification(string title, string message, byte[] largeIconBytes = null)
        {
            if (!isChannelInitialized)
            {
                CreateNotificationChannel();
                isChannelInitialized = true;
            }

            notificationId++;

            Bitmap largeIcon = largeIconBytes == null ? null
                : BitmapFactory.DecodeByteArray(largeIconBytes, 0, largeIconBytes.Length);

            var pendingIntent = PendingIntent.GetActivity(Application.Context, 
                notificationId, Platform.CurrentActivity.Intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                .SetContentText(message)
                .SetContentTitle(title)
                .SetLargeIcon(largeIcon)
                .SetSmallIcon(Resource.Mipmap.ic_white_notific)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetDefaults(NotificationCompat.DefaultAll);
            
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(notificationId, notificationBuilder.Build());
        }
    }
}