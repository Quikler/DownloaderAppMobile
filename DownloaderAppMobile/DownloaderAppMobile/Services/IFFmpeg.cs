using System.Threading;
using System.Threading.Tasks;
using DownloaderAppMobile.Models;

namespace DownloaderAppMobile.Services
{
    public interface IFFmpeg
    {
        Task ExecuteAsync(string command, CancellationToken cancellationToken = default);
        Task CreateAudioAsync(string destinationFilePath, string tempVideoFilePath,
            FFmpegOptions options, bool deleteTemp = false, CancellationToken cancellationToken = default);
        Task CreateVideoAsync(string destinationFilePath, string tempVideoFilePath,
            FFmpegOptions options, bool deleteTemp = false, CancellationToken cancellationToken = default);
    }
}
