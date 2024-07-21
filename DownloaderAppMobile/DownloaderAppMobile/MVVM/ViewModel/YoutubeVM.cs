using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DownloaderAppMobile.Models;
using DownloaderAppMobile.Helpers;
using DownloaderAppMobile.Services;
using DownloaderAppMobile.MVVM.Model;
using DownloaderAppMobile.MVVM.Abstractions;
using Xamarin.Forms;
using Xamarin.Essentials;
using YoutubeExplode.Videos;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace DownloaderAppMobile.MVVM.ViewModel
{
    public class YoutubeVM : InputOutputVM
    {
        public List<MediaTypeItem> MediaTypes { get; }
        public YoutubeModel Model { get; }
        
        private int _selectedMediaTypeIndex;
        public int SelectedMediaTypeIndex
        {
            get { return _selectedMediaTypeIndex; }
            set { RaiseAndSetIfChanged(ref _selectedMediaTypeIndex, value); }
        }

        private string _mediaSource;
        public string MediaSource
        {
            get { return _mediaSource; }
            set { RaiseAndSetIfChanged(ref _mediaSource, value); }
        }

        private bool _isPickerEnabled;
        public bool IsPickerEnabled
        {
            get { return _isPickerEnabled; }
            set { RaiseAndSetIfChanged(ref _isPickerEnabled, value); }
        }

        public YoutubeVM()
        {
            Model = new YoutubeModel();
            IsPickerEnabled = true;

            MediaTypes = new List<MediaTypeItem>
            {
                new MediaTypeItem { Title = "Audio", MediaType = MediaType.Audio, },
                new MediaTypeItem { Title = "Video", MediaType = MediaType.Video, },
            };

            SelectedMediaTypeIndex = 0;
        }

        protected override async void ActionButtonClickCommandExecute(object parameter)
        {
            SetProperties(false);

            try
            {
                IStreamInfo streamInfo;
                Video video;
                StreamManifest manifest;

                if (Model.Info.HasValue && EntryText.Contains(Model.Info.Value.video.Id))
                {
                    video = Model.Info.Value.video;
                    manifest = Model.Info.Value.manifest;
                    streamInfo = Model.StreamInfo;
                }
                else
                {
                    Model.Info = await Task.Run(() => YoutubeHelper.GetInfoAsync(EntryText));
                    if (!Model.Info.HasValue)
                    {
                        SetProperties(true);
                        return;
                    }

                    video = Model.Info.Value.video;
                    manifest = Model.Info.Value.manifest;

                    streamInfo = Model.StreamInfo = manifest.GetMuxedStreams().GetWithHighestVideoQuality();
                    MediaSource = streamInfo.Url;
                }

                bool isAudio = MediaTypes[SelectedMediaTypeIndex].MediaType == MediaType.Audio;
                if (isAudio)
                {
                    streamInfo = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                }

                string tempVideoFilePath = await YoutubeHelper.DownloadToTempAsync(streamInfo);

                (string mediaType, string folder, string extenstion, FFmpegOptions options) =
                    await PrepareDownloadOptionsAsync(video, isAudio);

                string destinationFilePath = Path.Combine(folder, $"{options.Title}{extenstion}");

                IFFmpeg ffmpeg = DependencyService.Get<IFFmpeg>();
                Task createMediaTask = isAudio
                    ? ffmpeg.CreateAudioAsync(destinationFilePath, tempVideoFilePath, options, true)
                    : ffmpeg.CreateVideoAsync(destinationFilePath, tempVideoFilePath, options, true);
                await createMediaTask;

                DependencyService.Get<IFileService>().ScanFile(destinationFilePath);
                _ = Page.DisplayAlert(Phrases.YOUTUBE, $"{mediaType} downloaded: \"{video.Title}\"", Phrases.OK);
                DependencyService.Get<ILocalNotificationsService>()
                    .ShowNotification(video.Title, $"{mediaType} downloaded", options.Thumbnail);
            }
            catch (Exception ex)
            {
                _ = Page.DisplayAlert(Phrases.YOUTUBE, ex.Message, Phrases.OK);
            }
            finally
            {
                SetProperties(true);
            }
        }

        protected override bool ActionButtonClickCommandCanExecute(object parameter)
            => !string.IsNullOrWhiteSpace(EntryText) && YoutubeModel.Regex.IsMatch(EntryText);

        protected override async void OpenSocialMediaCommandExecute(object parameter)
            => await Launcher.OpenAsync("https://www.youtube.com");

        private void SetProperties(bool state)
        {
            IsActivityIndicatorRunning = !state;
            IsActionButtonEnabled = state;
            IsPickerEnabled = state;
        }

        private async Task<(string mediaType, string folder, string extension, FFmpegOptions options)> PrepareDownloadOptionsAsync(
            Video video, bool isAudio)
        {
            string mediaType = isAudio ? "Audio" : "Video";
            string extension = isAudio ? ".mp3" : ".mp4";

            var folderService = DependencyService.Get<IFolderService>();
            string folder = isAudio ?
                folderService.CreateFolder("Music", "DownloaderApp") :
                folderService.CreateFolder("Movies", "DownloaderApp");

            var thumbnail = await YoutubeHelper.GetThumbnailBytesAsync(video.Thumbnails.GetWithHighestResolution().Url);
            var options = new FFmpegOptions
            {
                Author = PathHelper.CreateValidFileName(video.Author.ChannelTitle),
                Title = PathHelper.CreateValidFileName(video.Title),
                Date = video.UploadDate.Year,
                Album = "DownloaderApp",
                AlbumArtist = "Quikler",
                Thumbnail = thumbnail,
            };

            return (mediaType, folder, extension, options);
        }
    }
}
