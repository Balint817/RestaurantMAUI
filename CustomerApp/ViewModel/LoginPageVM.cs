using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Helpers;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.ViewModel
{
    public partial class LoginPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
#pragma warning disable CS8618
        public LoginPageVM()
        {
            LoginCommand = new Command(OnLoginButtonClicked);
            GoToRegisterCommand = new Command(OnGoRegisterClicked);
            GoogleLoginCommand = new Command(OnGoogleLoginClicked);
            ForgotPasswordCommand = new Command(OnForgotPasswordClicked);
        }

        private async void OnGoogleLoginClicked(object obj)
        {
            //App.Current!.MainPage = new GoogleLogin();
            await DoGoogleLogin().MakeTaskBlocking(Page);
        }

        private async Task DoGoogleLogin()
        {
            //// Launch login in system browser
            //await Browser.OpenAsync("https://mateszadam.koyeb.app/user/google", BrowserLaunchMode.External);

            //// Meanwhile, wait a second and navigate to catcher
            //// So once the browser redirects, user can return manually
            //await Task.Delay(1000);
            //await Application.Current.MainPage.Navigation.PushAsync(new GoogleLogin());
        }

        private void OnForgotPasswordClicked(object obj)
        {
            App.Current!.MainPage = new ForgotPasswordPage();
        }
#pragma warning restore CS8618

        private void OnGoRegisterClicked(object obj)
        {
            App.Current!.MainPage = new RegisterPage();
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private string? _usernameEntry;
        public string? UsernameEntry
        {
            get { return _usernameEntry; }
            set { _usernameEntry = value; OnPropertyChanged(); ErrorMessage = null!; }
        }

        private string? _passwordEntry;
        public string? PasswordEntry
        {
            get { return _passwordEntry; }
            set { _passwordEntry = value; OnPropertyChanged(); ErrorMessage = null!; }
        }

        public Command LoginCommand { get; }
        public Command GoToRegisterCommand { get; }
        public Command GoogleLoginCommand { get; }
        public Command ForgotPasswordCommand { get; }
        public LoginPage Page { get; internal set; }

        bool loading;
        internal async void OnLoginButtonClicked()
        {
            if (!loading)
            {
                loading = true;
                await DoLogin().MakeTaskBlocking(Page);
                loading = false;
            }
            else
            {
                // OBSOLETE

                //if (ErrorMessage?.StartsWith("Kérem várjon!") == true)
                //{
                //    ErrorMessage += "!";
                //}
                //else
                //{
                //    ErrorMessage = "Kérem várjon!";
                //}
            }
        }

        async Task DoLogin()
        {

            var username = UsernameEntry;
            var password = PasswordEntry;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = LanguageService["FillOutAllFields"].Current;
                return;
            }
            if (AuthService.CheckUsername(username) is string userError)
            {
                ErrorMessage = userError;
                return;
            }
            if (AuthService.CheckPassword(password) is string passError)
            {
                ErrorMessage = passError;
                return;
            }

            ErrorMessage = "";

            try
            {
                var result = await AuthService.Instance.Login(username, password);
                switch (result)
                {
                    case true:
                        await ((App)App.Current!).Init();
                        break;
                    case false:
                        ErrorMessage = LanguageService["IncorrectUser"].Current;
                        break;
                    default:
                        ErrorMessage = LanguageService["ServerError"].Current;
                        break;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                await App.Current!.MainPage!.DisplayAlert(LanguageService["ErrorTitle"].Current, ex.ToString(), "OK");
#endif
                var cause = ex.GetHttpExceptionCause();
                switch (cause)
                {
                    case HttpExceptionCause.NoInternet:
                        ErrorMessage = LanguageService["NoInternet"].Current;
                        break;
                    case HttpExceptionCause.RequestLost:
                        ErrorMessage = LanguageService["Timeout"].Current;
                        break;
                    case HttpExceptionCause.NotAnHttpException:
                    case HttpExceptionCause.Unknown:
                    default:
                        ErrorMessage = LanguageService["UnknownError"].Current;
                        break;
                }
            }
        }
    }
}
