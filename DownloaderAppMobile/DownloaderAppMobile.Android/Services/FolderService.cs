using Android.Media;
using System.IO;
using DownloaderAppMobile.Droid.Services;
using DownloaderAppMobile.Services;

[assembly: Xamarin.Forms.Dependency(typeof(FolderService))]
namespace DownloaderAppMobile.Droid.Services
{
    public class FolderService : IFolderService
    {
        public string CreateFolder(string publicDir, string folderName, bool scanOnCreate = true)
        {
            var collection = Android.OS.Environment.GetExternalStoragePublicDirectory(publicDir);
            var folderPath = Path.Combine(collection.AbsolutePath, folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);

                if (scanOnCreate)
                {
                    MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { folderPath }, null, null);
                }
            }

            return folderPath;
        }
    }
}