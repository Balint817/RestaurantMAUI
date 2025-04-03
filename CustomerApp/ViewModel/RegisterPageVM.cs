using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.ViewModel
{
    public class RegisterPageVM: BindableObject
    {
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
        public RegisterPageVM()
        {
            GoToLoginCommand = new Command(OnGoLoginClicked);
            RegisterCommand = new Command(OnRegisterButtonClicked);
        }
        bool loading;
        internal async void OnRegisterButtonClicked()
        {
            if (!loading)
            {
                loading = true;
                await DoRegister();
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

        async Task DoRegister()
        {

            var username = UsernameEntry;
            var password = PasswordEntry;
            var email = EmailEntry;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {
                ErrorMessage = "Töltsön ki minden mezőt!";
                return;
            }

            try
            {
                var result = await AuthService.Instance.Register(username, password, email);
                switch (result.Key)
                {
                    case true:
                        ErrorMessage = "";
                        await ((App)App.Current!).Init();
                        break;
                    case false:
                        ErrorMessage = result.Value ?? "Hiba történt.";
                        break;
                    default:
                        ErrorMessage = result.Value ?? "Szerver hiba. Próbálja meg újra!";
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
