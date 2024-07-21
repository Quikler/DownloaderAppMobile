using Android.Media;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using DownloaderAppMobile.Droid.Services;
using DownloaderAppMobile.Services;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace DownloaderAppMobile.Droid.Services
{
    public class FileService : IFileService
    {
        public void ScanFile(string filePath)
        {
            MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { filePath }, null, null);
        }

        public async Task RequestPermissionAsync<T>() where T : Permissions.BasePermission
        {
            var permission = Activator.CreateInstance<T>();
            if ((await permission.CheckStatusAsync()) == PermissionStatus.Granted)
                return;

            await permission.RequestAsync();
            permission.EnsureDeclared();
        }
    }
}