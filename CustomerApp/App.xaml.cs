using CustomerApp.Resources.Styles;
using CustomerApp.Services;
using CustomerApp.View;
using CustomerApp.ViewModel;

namespace CustomerApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Application.Current.RequestedThemeChanged += (s, a) =>
            {
                var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
                if (mergedDictionaries is null)
                {
                    return;
                }
                mergedDictionaries.Remove(mergedDictionaries.First(x => x is Dummy_ThemeColorInterface));
                switch (a.RequestedTheme)
                {
                    case AppTheme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    case AppTheme.Unspecified:
                    case AppTheme.Light:
                    default:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }
            };

            MainPage = new LoginPage();

            Init();
        }

        private async void Init()
        {
            var authService = AuthService.Instance;

            var isAuth = await authService.Init();
            if (isAuth)
            {
                MainPage = new AppShell();
            }
        }
    }
}
