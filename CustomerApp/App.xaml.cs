using System.Diagnostics;
using System.Linq;
using System.Text;
using CustomerApp.Resources.Styles;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp
{
    public partial class App : Application
    {
        public static async Task ShowGenericNetworkError()
        {
            await App.Current!.MainPage!.DisplayAlert(LanguageService.Instance["ErrorTitle"].Current, LanguageService.Instance["PageLoadError"].Current, LanguageService.Instance["OK"].Current);
        }
        void AddTheme(AppTheme requestedTheme)
        {
            var mergedDictionaries = App.Current!.Resources.MergedDictionaries;
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
        public static string GetFakeExceptionStackTrace(string exceptionType = "System.FakeException", string message = "No error.")
        {
            var trace = new StackTrace(true); // true to capture file info
            var frames = trace
                .GetFrames()
                .Skip(1) // skips the current method
                .ToArray();

            var sb = new StringBuilder();
            sb.AppendLine($"{exceptionType}: {message}");

            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    var declaringType = method?.DeclaringType;
                    var methodName = method?.Name;
                    var fileName = frame.GetFileName();
                    var lineNumber = frame.GetFileLineNumber();

                    sb.Append("   at ");
                    if (declaringType != null)
                    {
                        sb.Append($"{declaringType.FullName}.{methodName}");
                    }
                    else
                    {
                        sb.Append(methodName ?? "<UnknownMethod>");
                    }

                    if (!string.IsNullOrEmpty(fileName) && lineNumber > 0)
                    {
                        sb.Append($" in {fileName}:line {lineNumber}");
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
        public static void PrintNavigationDetails(INavigation? navigation = null)
        {
            Console.WriteLine($"!!!!!!printing navigation details ({navigation is null}) ({DateTime.UtcNow})");
            navigation ??= GetNavigation();

            var stack = navigation.NavigationStack;
            var stackNames = stack
                .Select(x => x?.GetType().Name ?? "<null>")
                .ToArray();
            Console.WriteLine($"current navigation: [{string.Join(", ", stackNames)}]");

            Console.WriteLine($"call stack trace:\n{GetFakeExceptionStackTrace()}");

            Console.WriteLine($"--------------------");
        }
        public static INavigation GetNavigation()
        {
            var navigation = ((App)App.Current!).MainPage!.Navigation;
#if DEBUG
            //PrintNavigationDetails(navigation);
#endif
            return navigation;
        }
        public App()
        {
            InitializeComponent();

            AddTheme(App.Current!.RequestedTheme);

            App.Current!.RequestedThemeChanged += (s, a) =>
            {
                var mergedDictionaries = App.Current!.Resources.MergedDictionaries;
                if (mergedDictionaries is null)
                {
                    return;
                }
                mergedDictionaries.Remove(mergedDictionaries.First(x => x is Dummy_ThemeColorInterface));
                AddTheme(a.RequestedTheme);
            };

            MainPage = new EmptyPage();

#pragma warning disable CS4014 // Can't await here because this is a constructor.
            Init();
#pragma warning restore CS4014 
        }

        public async Task Init()
        {
            MainPage = await AuthService.Instance.Init()
                ? new AppShell()
                : new LoginPage();
        }
    }
}
