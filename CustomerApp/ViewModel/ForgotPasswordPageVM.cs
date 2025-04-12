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
    public partial class ForgotPasswordPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
#pragma warning disable CS8618
        public ForgotPasswordPageVM()
        {
            ForgotPasswordCommand = new Command(OnButtonClicked);
            GoToLoginCommand = new Command(OnGoLoginClicked);
        }
#pragma warning restore CS8618

        private void OnGoLoginClicked(object obj)
        {
            App.Current!.MainPage = new LoginPage();
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private string? _emailEntry;
        public string? EmailEntry
        {
            get { return _emailEntry; }
            set { _emailEntry = value; OnPropertyChanged(); ErrorMessage = null!; }
        }
        public Command ForgotPasswordCommand { get; }
        public Command GoToLoginCommand { get; }
        public ForgotPasswordPage Page { get; internal set; }

        bool loading;
        internal async void OnButtonClicked()
        {
            if (!loading)
            {
                loading = true;
                await DoPasswordReset().MakeTaskBlocking(Page);
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

        async Task DoPasswordReset()
        {

            var email = EmailEntry;

            if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessage = LanguageService["FillOutAllFields"].Current;
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
                var result = await AuthService.Instance.ForgotPassword(email);
                switch (result)
                {
                    case true:
                        await Page.DisplayAlert(LanguageService["Success"].Current, LanguageService["PasswordResetSuccess"].Current, LanguageService["OK"].Current);
                        GoToLoginCommand.Execute(null);
                        break;
                    case false:
                        ErrorMessage = LanguageService["InvalidEmail"].Current;
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
