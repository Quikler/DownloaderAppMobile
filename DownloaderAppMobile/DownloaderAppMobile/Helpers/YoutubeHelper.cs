using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace DownloaderAppMobile.Helpers
{
    public static class YoutubeHelper
    {
        public static YoutubeClient Client { get; } = new YoutubeClient();

        public static async Task<(Video, StreamManifest)> GetInfoAsync(string url, CancellationToken cancellationToken = default)
        {
            Video video = await Client.Videos.GetAsync(url, cancellationToken);
            StreamManifest manifest = await Client.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
            return (video, manifest);
        }

        public static async Task<StreamManifest> GetManifestAsync(string url)
            => await Client.Videos.Streams.GetManifestAsync(url);

        public static async Task<Video> GetVideoAsync(string url)
            => await Client.Videos.GetAsync(url);

        public static async Task<byte[]> GetThumbnailBytesAsync(string thumbnailUrl)
        {
            using (HttpClient hc = new HttpClient())
            {
                return await hc.GetByteArrayAsync(thumbnailUrl);
            }
        }

        public static async Task<string> DownloadToTempAsync(
            IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        {
            string temp = Path.GetTempFileName();
            await Client.Videos.Streams.DownloadAsync(streamInfo, temp, cancellationToken: cancellationToken);
            return temp;
        }
    }
}
