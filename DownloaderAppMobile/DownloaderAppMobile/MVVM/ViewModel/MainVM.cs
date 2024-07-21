using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using DownloaderAppMobile.MVVM.Abstractions;
using DownloaderAppMobile.MVVM.Model;

namespace DownloaderAppMobile.MVVM.ViewModel
{
    public class MainVM : BaseVM
    {
        private CarouselPage _currentDetailPage;
        public CarouselPage CurrentDetailPage
        {
            get { return _currentDetailPage; }
            set { RaiseAndSetIfChanged(ref _currentDetailPage, value); }
        }

        private bool _isFlyoutPresented;
        public bool IsFlyoutPresented
        {
            get { return _isFlyoutPresented; }
            set { RaiseAndSetIfChanged(ref _isFlyoutPresented, value); }
        }

        public ICommand CurrentPageChangedCommand { get; }

        public MainVM()
        {
            // Subscribe to message from MainPage.xaml.cs to initialize first detail page
            MessagingCenter.Subscribe<MainPage, Page>(this, "DetailPageFromMainPage", (sender, e) =>
                CurrentDetailPage = (CarouselPage)e
            );

            CurrentPageChangedCommand = new Command(() =>
                MessagingCenter.Send(this, "CurrentPageChangedCommand", CurrentDetailPage.CurrentPage)
            );
            
            // MainVM subscrives to FlyoutVM send selected FlyoutMenuItem from ListView
            MessagingCenter.Subscribe<FlyoutVM, FlyoutMenuItem>(this, "MenuItemSelectedCommand", (sender, e) =>
            {
                CurrentDetailPage.CurrentPage = CurrentDetailPage.Children.First(cp => cp.GetType() == e.PageType);
                IsFlyoutPresented = false;
            });
        }
    }
}
