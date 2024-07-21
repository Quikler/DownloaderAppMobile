using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using DownloaderAppMobile.MVVM.View;
using DownloaderAppMobile.MVVM.Model;
using DownloaderAppMobile.MVVM.Abstractions;

namespace DownloaderAppMobile.MVVM.ViewModel
{
    public class FlyoutVM : BaseVM
    {
        public ObservableCollection<FlyoutMenuItem> Menu { get; }
        public ICommand MenuItemSelectedCommand { get; }

        private FlyoutMenuItem _selectedItem;
        public FlyoutMenuItem SelectedItem
        {
            get { return _selectedItem; }
            set { RaiseAndSetIfChanged(ref _selectedItem, value); }
        }

        public FlyoutVM()
        {
            Menu = new ObservableCollection<FlyoutMenuItem>
            {
                new FlyoutMenuItem { ImageSource = "youtube", Title = "Youtube", PageType = typeof(YoutubeView), },
                new FlyoutMenuItem { ImageSource = "instagram", Title = "Instagram", PageType = typeof(InstagramView), },
                new FlyoutMenuItem { ImageSource = "login", Title = "Login Instagram", PageType = typeof(InstagramLoginView), },
            }; 

            SelectedItem = Menu.First();

            // Send selected menu item
            MenuItemSelectedCommand = new Command(() =>
                MessagingCenter.Send(this, "MenuItemSelectedCommand", SelectedItem)
            );

            // Subscribe to CurrentPageChangedCommand message to synchronize SelectedItem and CarouselPage.CurrentPage
            MessagingCenter.Subscribe<MainVM, ContentPage>(this, "CurrentPageChangedCommand", (sender, e) =>
                SelectedItem = Menu.First(m => m.PageType == e.GetType())
            );
        }
    }
}
