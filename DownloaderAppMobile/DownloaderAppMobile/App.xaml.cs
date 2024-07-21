using System.Linq;
using Xamarin.Forms;
using DownloaderAppMobile.MVVM.Model;
using DownloaderAppMobile.MVVM.View;
using DownloaderAppMobile.MVVM.ViewModel;

namespace DownloaderAppMobile
{
    public partial class App : Application
    {
        public static bool IsForeground { get; private set; }
        public string ExtraText { get; set; }

        public App()
        {
            InitializeComponent();

            PageAppearing += InitExtraText;

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Resources.TryGetValue("DarkTertiaryColor", out var color) ? (Color)color : Color.DarkBlue,
            };
        }

        private void InitExtraText(object sender, Page e)
        {
            if (ExtraText != null && e is MainPage)
            {
                var mainVm = (MainVM)e.BindingContext;
                ContentPage contentPage = mainVm.CurrentDetailPage.CurrentPage;

                if (YoutubeModel.Regex.IsMatch(ExtraText))
                {
                    contentPage = mainVm.CurrentDetailPage.Children.First(cp => cp.GetType() == typeof(YoutubeView));
                    var youtubeVm = (YoutubeVM)contentPage.BindingContext;

                    var match = YoutubeModel.Regex.Match(ExtraText);
                    youtubeVm.EntryText = match.Value;
                }
                else if (InstagramModel.Regex.IsMatch(ExtraText))
                {
                    contentPage = mainVm.CurrentDetailPage.Children.First(cp => cp.GetType() == typeof(InstagramView));
                    var instagramVm = (InstagramVM)contentPage.BindingContext;

                    var match = InstagramModel.Regex.Match(ExtraText);
                    instagramVm.EntryText = match.Value;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    mainVm.CurrentDetailPage.CurrentPage = contentPage;
                });

                ExtraText = null;
            }
        }

        protected override void OnStart()
        {
            IsForeground = true;
        }

        protected override void OnSleep()
        {
            IsForeground = false;
        }

        protected override void OnResume()
        {
            IsForeground = true;
        }
    }
}
