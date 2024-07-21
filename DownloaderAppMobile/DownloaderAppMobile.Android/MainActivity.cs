using Android.OS;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;
using AndroidX.Core.App;

namespace DownloaderAppMobile.Droid
{
    // <activity>
    [Activity(
        Label = "DownloaderApp", 
        Icon = "@mipmap/ic_launcher", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        LaunchMode = LaunchMode.SingleInstance, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    // <intent-filter>
    [IntentFilter(
        actions: new string[] { Intent.ActionSend },
        Categories = new string[] { Intent.CategoryDefault },
        DataMimeType = "text/plain")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CancelAllNotifications();
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            App app = new App();
            LoadApplication(app);
            
            // Intent url
            if (Intent.ActionSend.Equals(Intent.Action) 
                && Intent.Type != null && "text/plain".Equals(Intent.Type))
            {
                app.ExtraText = Intent.GetStringExtra(Intent.ExtraText);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            CancelAllNotifications();
            base.OnNewIntent(intent);
            (Xamarin.Forms.Application.Current as App).ExtraText = intent.GetStringExtra(Intent.ExtraText);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void CancelAllNotifications() => NotificationManagerCompat.From(this).CancelAll();
    }
}