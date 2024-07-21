using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DownloaderAppMobile.Services
{
    public interface IFileService
    {
        void ScanFile(string filePath);
        Task RequestPermissionAsync<T>() where T : Permissions.BasePermission;
    }
}
