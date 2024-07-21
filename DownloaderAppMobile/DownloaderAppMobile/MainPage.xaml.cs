using System;
using Xamarin.Forms;
using DownloaderAppMobile.MVVM.ViewModel;

namespace DownloaderAppMobile
{
    public partial class MainPage : FlyoutPage
    {
        public MainPage()
        {
            InitializeComponent();
            MessagingCenter.Send(this, "DetailPageFromMainPage", Detail);
        }
    }
}
