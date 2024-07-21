using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace DownloaderAppMobile.MVVM.Abstractions.Interfaces
{
    public interface IInputOutputVM
    {
        ICommand EntryTextChangedCommand { get; }
        ICommand ActionButtonClickCommand { get; }
        ICommand ClipboardClickCommand { get; }
        ICommand OpenSocialMediaCommand { get; }

        Page Page { get; }

        string EntryText { get; set; }
        bool IsActionButtonEnabled { get; set; }
        bool IsActivityIndicatorRunning { get; set; }
    }
}
