using System.Windows.Input;
using Xamarin.Forms;
using DownloaderAppMobile.MVVM.Abstractions.Interfaces;
using Xamarin.Essentials;

namespace DownloaderAppMobile.MVVM.Abstractions
{
    public abstract class InputOutputVM : BaseVM, IInputOutputVM
    {
        public virtual ICommand EntryTextChangedCommand { get; }
        public virtual ICommand ActionButtonClickCommand { get; }
        public virtual ICommand ClipboardClickCommand { get; }
        public virtual ICommand OpenSocialMediaCommand { get; }

        protected virtual void EntryTextChangedCommandExecute(object parameter)
            => (ActionButtonClickCommand as Command)?.ChangeCanExecute();
        protected virtual void ActionButtonClickCommandExecute(object parameter) { }
        protected virtual async void ClipboardClickCommandExecute(object parameter) 
            => EntryText = await Clipboard.GetTextAsync();
        protected virtual void OpenSocialMediaCommandExecute(object parameter) { }
        protected virtual bool ActionButtonClickCommandCanExecute(object parameter) => true;
        protected virtual bool EntryTextChangedCommandCanExecute(object parameter) => true;
        protected virtual bool ClipboardClickCommandCanExecute(object parameter) => true;
        protected virtual bool OpenSocialMediaCommandCanExecute(object parameter) => true;

        public virtual Page Page { get; set; }

        protected InputOutputVM()
        {
            ActionButtonClickCommand = new Command(ActionButtonClickCommandExecute, ActionButtonClickCommandCanExecute);
            EntryTextChangedCommand = new Command(EntryTextChangedCommandExecute, EntryTextChangedCommandCanExecute);
            ClipboardClickCommand = new Command(ClipboardClickCommandExecute, ClipboardClickCommandCanExecute);
            OpenSocialMediaCommand = new Command(OpenSocialMediaCommandExecute, OpenSocialMediaCommandCanExecute);

            Application.Current.PageAppearing += (sender, e) =>
            {
                if (e is MainPage && Page is null)
                    Page = e;
            };
        }

        private string _entryText;
        public virtual string EntryText
        {
            get { return _entryText; }
            set { RaiseAndSetIfChanged(ref _entryText, value); }
        }

        private bool _isDownloadButtonEnabled;
        public virtual bool IsActionButtonEnabled
        {
            get { return _isDownloadButtonEnabled; }
            set { RaiseAndSetIfChanged(ref _isDownloadButtonEnabled, value); }
        }

        private bool _isActivityIndicatorRunning;
        public bool IsActivityIndicatorRunning
        {
            get { return _isActivityIndicatorRunning; }
            set { RaiseAndSetIfChanged(ref _isActivityIndicatorRunning, value); }
        }
    }
}
