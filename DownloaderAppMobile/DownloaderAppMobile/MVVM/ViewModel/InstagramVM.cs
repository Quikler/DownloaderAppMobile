using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.UI.Views;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes;
using InstagramService.Classes.Models;
using DownloaderAppMobile.Services;
using DownloaderAppMobile.MVVM.View;
using DownloaderAppMobile.MVVM.Model;
using DownloaderAppMobile.MVVM.Abstractions;
using DownloaderAppMobile.Helpers;

namespace DownloaderAppMobile.MVVM.ViewModel
{
    public class InstagramVM : InputOutputVM
    {
        public InstagramModel Model { get; }

        private string _downloadButtonText;
        public string DownloadButtonText
        {
            get { return _downloadButtonText; }
            set { RaiseAndSetIfChanged(ref _downloadButtonText, value); }
        }

        private ObservableCollection<Xamarin.Forms.View> _itemsSource;
        public ObservableCollection<Xamarin.Forms.View> ItemsSource
        {
            get { return _itemsSource; }
            set { RaiseAndSetIfChanged(ref _itemsSource, value); }
        }

        private ObservableCollection<Xamarin.Forms.View> _selectedViews;
        public ObservableCollection<Xamarin.Forms.View> SelectedViews
        {
            get { return _selectedViews; }
            set { RaiseAndSetIfChanged(ref _selectedViews, value); }
        }

        public InstagramVM()
        {
            DownloadButtonText = Phrases.PREVIEW;
            ItemsSource = new ObservableCollection<Xamarin.Forms.View>();
            Model = new InstagramModel();

            Application.Current.PageAppearing += (sender, e) =>
            {
                if (e is InstagramLoginView)
                    MessagingCenter.Send(this, "InstagramModel", Model);
            };
        }

        protected override void EntryTextChangedCommandExecute(object parameter)
        {
            if (Model.Infos != null && Model.Infos.Count() > 0
                && !string.IsNullOrWhiteSpace(EntryText))
            {
                string initialUri = Model.Infos.First().InitialUri;
                DownloadButtonText = initialUri.Contains(EntryText) ||
                    EntryText.Contains(initialUri) ? Phrases.DOWNLOAD : Phrases.PREVIEW;
            }

            (ActionButtonClickCommand as Command)?.ChangeCanExecute();
        }

        protected override async void ActionButtonClickCommandExecute(object parameter)
        {
            if (!Model.InstagramService.Api.IsUserAuthenticated)
            {
                _ = Page.DisplayAlert(Phrases.INSTAGRAM, "User is not authenticated." +
                    " Please make sure you are logged in to your instagram account.", Phrases.OK);
                return;
            }

            SetProperties(false);

            IResult<bool> result;
            if (DownloadButtonText == Phrases.PREVIEW)
            {
                result = await LoadPreview(EntryText);
                DownloadButtonText = result.Succeeded ? Phrases.DOWNLOAD : Phrases.PREVIEW;
            }
            else
            {
                result = await DownloadSelected();
            }

            _ = Page.DisplayAlert(Phrases.INSTAGRAM, result.Info.Message, Phrases.OK);

            SetProperties(true);
        }

        protected override bool ActionButtonClickCommandCanExecute(object parameter)
            => !string.IsNullOrWhiteSpace(EntryText) && InstagramModel.Regex.IsMatch(EntryText);

        protected override async void OpenSocialMediaCommandExecute(object parameter)
            => await Launcher.OpenAsync("https://www.instagram.com");
        
        private async Task<IResult<bool>> LoadPreview(string url)
        {
            ItemsSource.Clear();

            var infosResult = await Model.InstagramService.MediaProcessor.GetInfosAsync(url);
            
            if (!infosResult.Succeeded && !await AcceptChallangeIfRequired(infosResult))
            {
                return Result.Fail(infosResult.Info.Message, false);
            }

            var infos = Model.Infos = infosResult.Value;
            for (int i = 0; i < infos.Count(); i++)
            {
                var info = infos.ElementAt(i);
                switch (info.MediaType)
                {
                    case InstaMediaType.Image:
                        var image = new Image
                        {
                            Source = info.Uri,
                            HeightRequest = 300,
                            WidthRequest = 300,
                        };
                        ItemsSource.Add(image);
                        break;

                    case InstaMediaType.Video:
                        var mediaElement = new MediaElement
                        {
                            Source = info.Uri,
                            HeightRequest = 300,
                            WidthRequest = 300,
                        };
                        ItemsSource.Add(mediaElement);
                        break;
                }
            }

            return Result.Success("Preview loaded successfully", true);
        }

        private async Task<IResult<bool>> DownloadSelected()
        {
            if (SelectedViews.Count <= 0)
            {
                return Result.Fail("Nothing selected. Make sure to select at least 1 media to download", false);
            }

            List<InstaMediaInfo> filteredInfos = new List<InstaMediaInfo>();
            foreach (var view in SelectedViews)
            {
                foreach (var info in Model.Infos)
                {
                    if ((view is Image image && image.Source.ToString().Contains(info.Uri))
                        || view is MediaElement media && media.Source.ToString().Contains(info.Uri))
                    {
                        filteredInfos.Add(info);
                    }
                }
            }

            var streamsResult = await Model.InstagramService.StreamProcessor.GetMediaStreamsAsync(filteredInfos);
            if (!streamsResult.Succeeded)
            {
                return Result.Fail(streamsResult.Info.Message, false);
            }

            var streams = streamsResult.Value;

            var folderService = DependencyService.Get<IFolderService>();
            string moviesFolder = folderService.CreateFolder("Movies", "DownloaderApp", true);
            string picturesFolder = folderService.CreateFolder("Pictures", "DownloaderApp", true);
            string suitableFolder;

            IFileService fileService = DependencyService.Get<IFileService>();
            await fileService.RequestPermissionAsync<Permissions.Photos>();
            await fileService.RequestPermissionAsync<Permissions.Media>();

            string guid = Guid.NewGuid().ToString();
            for (int i = 0; i < streams.Count(); i++)
            {
                var currentStream = streams.ElementAt(i);
                suitableFolder = currentStream.MediaType == InstaMediaType.Image ? picturesFolder : moviesFolder;

                string fileName = $"{currentStream.InstaMedia.Code}-{i}-{guid}";
                if (File.Exists(Path.Combine(suitableFolder, fileName)))
                    continue;

                try
                {
                    var fileInfo = await Model.InstagramService.MediaProcessor
                        .DownloadMediaAsync(currentStream, suitableFolder, fileName);
                    fileService.ScanFile(fileInfo.FullName);
                }
                catch (Exception ex)
                {
                    return Result.Fail(ex.Message, false);
                }
            }

            return Result.Success("All selected media has been downloaded", true);
        }

        private async Task<bool> AcceptChallangeIfRequired<T>(IResult<T> result)
        {
            if (result.Info.NeedsChallenge)
            {
                var acceptResult = await Model.InstagramService.Api.AcceptChallengeAsync();
                if (!acceptResult.Succeeded)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private void SetProperties(bool state)
        {
            IsActivityIndicatorRunning = !state;
            IsActionButtonEnabled = state;
        }
    }
}
