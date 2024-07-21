namespace DownloaderAppMobile.Models
{
    public class FFmpegOptions
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string AlbumArtist { get; set; }
        public string Album { get; set; }
        public byte[] Thumbnail { get; set; }
        public int Date { get; set; }

        public FFmpegOptions()
        {
            
        }

        public FFmpegOptions(string title, string author, string albumArtist,
            string album, byte[] thumbnail, int date)
        {
            Title = title;
            Author = author;
            AlbumArtist = albumArtist;
            Album = album;
            Thumbnail = thumbnail;
            Date = date;
        }
    }
}
