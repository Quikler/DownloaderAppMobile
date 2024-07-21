using System.Text.RegularExpressions;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace DownloaderAppMobile.MVVM.Model
{
    public class YoutubeModel
    {
        public static Regex Regex { get; } = new Regex(
                @"^(http(s)?://)?(www\.)?(youtube\.com/watch\?v=|youtu.be/|youtube.com/shorts/)([a-zA-Z0-9\-_]{11})((\?|&)\S*)?$",
                RegexOptions.IgnoreCase);

        public (Video video, StreamManifest manifest)? Info { get; set; }
        public IStreamInfo StreamInfo { get; set; }
    }
}
