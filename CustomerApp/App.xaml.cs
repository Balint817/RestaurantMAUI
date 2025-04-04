﻿using CustomerApp.Resources.Styles;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp
{
    public partial class App : Application
    {
        void AddTheme(AppTheme requestedTheme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            switch (requestedTheme)
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
        }
        public static INavigation GetNavigation() => ((App)App.Current!).MainPage!.Navigation;
        public App()
        {
            InitializeComponent();

            AddTheme(Application.Current.RequestedTheme);

            Application.Current!.RequestedThemeChanged += (s, a) =>
            {
                var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
                if (mergedDictionaries is null)
                {
                    return;
                }
                mergedDictionaries.Remove(mergedDictionaries.First(x => x is Dummy_ThemeColorInterface));
                AddTheme(a.RequestedTheme);
            };

            MainPage = new EmptyPage();

            Init();
        }

        public async Task Init()
        {
            MainPage = await AuthService.Instance.Init()
                ? new AppShell()
                : new LoginPage();
        }
    }
}
