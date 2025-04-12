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
    public partial class RegisterPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        private void OnGoLoginClicked(object obj)
        {
            App.Current!.MainPage = new LoginPage();
        }
        public Command RegisterCommand { get; }
        public Command GoToLoginCommand { get; }

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

        private string? _emailEntry;
        public string? EmailEntry
        {
            get { return _emailEntry; }
            set { _emailEntry = value; OnPropertyChanged(); ErrorMessage = null!; }
        }

        private string? _passwordEntry;
        public string? PasswordEntry
        {
            get { return _passwordEntry; }
            set { _passwordEntry = value; OnPropertyChanged(); ErrorMessage = null!; }
        }
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;

        public RegisterPage Page { get; internal set; }

#pragma warning disable CS8618
        public RegisterPageVM()
        {
            GoToLoginCommand = new Command(OnGoLoginClicked);
            RegisterCommand = new Command(OnRegisterButtonClicked);
        }
#pragma warning restore CS8618
        bool loading;
        internal async void OnRegisterButtonClicked()
        {
            if (!loading)
            {
                loading = true;
                await DoRegister().MakeTaskBlocking(Page);
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

        async Task DoRegister()
        {

            var username = UsernameEntry;
            var password = PasswordEntry;
            var email = EmailEntry;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
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
            if (AuthService.CheckEmail(email) is string emailError)
            {
                ErrorMessage = emailError;
                return;
            }
            ErrorMessage = "";

            try
            {
                var result = await AuthService.Instance.Register(username, password, email);
                switch (result.Key)
                {
                    case true:
                        await ((App)App.Current!).Init();
                        break;
                    case false:
                        ErrorMessage = result.Value ?? LanguageService["UnknownError"].Current;
                        break;
                    default:
                        ErrorMessage = result.Value ?? LanguageService["ServerError"].Current;
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
