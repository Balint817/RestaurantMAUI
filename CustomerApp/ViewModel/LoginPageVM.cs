using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.ViewModel
{
    public class LoginPageVM: BindableObject
    {
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public LoginPageVM()
        {
            LoginCommand = new Command(OnLoginButtonClicked);
            GoToRegisterCommand = new Command(OnGoRegisterClicked);
        }

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

        bool loading;
        internal async void OnLoginButtonClicked()
        {
            if (!loading)
            {
                loading = true;
                await DoLogin();
                loading = false;
            }
            else
            {
                if (ErrorMessage?.StartsWith("Kérem várjon!") == true)
                {
                    ErrorMessage += "!";
                }
                else
                {
                    ErrorMessage = "Kérem várjon!";
                }
            }
        }

        async Task DoLogin()
        {

            var username = UsernameEntry;
            var password = PasswordEntry;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Töltsön ki minden mezőt!";
                return;
            }

            try
            {
                var result = await AuthService.Instance.Login(username, password);
                switch (result)
                {
                    case true:
                        ErrorMessage = "";
                        await ((App)App.Current!).Init();
                        break;
                    case false:
                        ErrorMessage = "Rossz jelszó vagy felhasználónév!";
                        break;
                    default:
                        ErrorMessage = "Szerver hiba. Próbálja meg újra!";
                        break;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                await App.Current!.MainPage!.DisplayAlert("Error", ex.ToString(), "K");
#endif
                ErrorMessage = "Hiba történt.";
            }
        }
    }
}
