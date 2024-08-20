using DownloaderAppMobile.Helpers;
using DownloaderAppMobile.MVVM.Abstractions;
using DownloaderAppMobile.MVVM.Model;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramService.Classes;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DownloaderAppMobile.MVVM.ViewModel
{
    public class InstagramLoginVM : InputOutputVM
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { RaiseAndSetIfChanged(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { RaiseAndSetIfChanged(ref _password, value); }
        }

        private string _loginState;
        public string LoginState
        {
            get { return _loginState; }
            set { RaiseAndSetIfChanged(ref _loginState, value); }
        }

        private InstagramModel _instagramModel;

        public InstagramLoginVM()
        {
            MessagingCenter.Subscribe<InstagramVM, InstagramModel>(this, "InstagramModel", (sender, e) =>
            {
                if (_instagramModel != null)
                    return;

                _instagramModel = e;
                LoginState = _instagramModel.InstagramService.Api.IsUserAuthenticated ? "Logged" : "Not logged";
                (ActionButtonClickCommand as Command).ChangeCanExecute();
            });

            Application.Current.PageAppearing += (sender, e) =>
            {
                if (e is MainPage && Page is null)
                    Page = e;
            };
        }

        protected override bool ActionButtonClickCommandCanExecute(object parameter)
            => Password != null && Username != null && Password.Length >= 6 && Username.Length >= 1;

        protected override async void ActionButtonClickCommandExecute(object parameter)
        {
            SetProperties(false);

            var data = File.ReadAllText(_instagramModel.InstagramService.Api.SessionHandler.FilePath);
            
            // If session data is not empty load it
            if (data.Length != 0)
                _instagramModel.InstagramService.Api.SessionHandler.Load(false);

            (Username, Password) = (Username.Trim(), Password.Trim());

            bool accept;
            if (_instagramModel.InstagramService.Api.IsUserAuthenticated)
            {
                accept = await Page.DisplayAlert(Phrases.INSTAGRAM, 
                    "You already authenticated are you sure you want to change your account?", Phrases.YES, Phrases.NO);

                if (!accept)
                {
                    _ = Page.DisplayAlert(Phrases.INSTAGRAM, "New login operation was canceled", Phrases.OK);
                    SetProperties(true);
                    return;
                }
            }

            var loginResult = await LoginAsync(Username, Password);
            if (loginResult.Succeeded)
            {
                LoginState = "Logged";
            }
            _ = Page.DisplayAlert(Phrases.INSTAGRAM, loginResult.Info.Message, Phrases.OK);

            SetProperties(true);
        }

        private void SetProperties(bool state)
        {
            IsActivityIndicatorRunning = !state;
            IsActionButtonEnabled = state;
        }

        private async Task<IResult<bool>> LoginAsync(string username, string password)
        {
            IInstaApi instaApi = InstaApiBuilder
                .CreateBuilder()
                .SetSessionHandler(new FileSessionHandler { FilePath = InstagramModel.AccountSessionFilePath })
                .SetUser(new UserSessionData { UserName = username, Password = password })
                .Build();

            await instaApi.SendRequestsBeforeLoginAsync();

            var loginResult = await instaApi.LoginAsync();

            if (!loginResult.Succeeded)
            {
                return Result.Fail(loginResult.Info, false);
            }

            await instaApi.SendRequestsAfterLoginAsync();

            instaApi.SessionHandler.Save(false);
            _instagramModel.InstagramService = new InstaService(instaApi);
            return Result.Success("User has successfully logged in", true);
        }
    }
}
