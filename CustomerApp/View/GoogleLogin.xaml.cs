using System.Text.RegularExpressions;
using CustomerApp.Services;

namespace CustomerApp.View;

public partial class GoogleLogin : ContentPage
{
    private static readonly Regex RedirectRegex = RedirectRegexCompiled();
    private static LanguageService LanguageService => LanguageService.Instance;
    public GoogleLogin()
    {
        InitializeComponent();
        ClearWebViewData();
        LoginWebView.Source = "https://mateszadam.koyeb.app/user/google";
    }
    private void ClearWebViewData()
    {
#if ANDROID
        var webView = new Android.Webkit.WebView(Android.App.Application.Context);
        if (Android.Webkit.CookieManager.Instance is {} cookieManager) {
            cookieManager.RemoveAllCookies(null);
            cookieManager.Flush();
        }
        webView.ClearCache(true);
        webView.ClearFormData();
        webView.ClearHistory();
#elif IOS
        var websiteDataTypes = new Foundation.NSSet<Foundation.NSString>(WebKit.WKWebsiteDataStore.AllWebsiteDataTypes);
        var dateFrom = Foundation.NSDate.DistantPast;
        WebKit.WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(websiteDataTypes, dateFrom, () => { });
#endif
    }

    bool firstLoad = true;
    private async void LoginWebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (firstLoad)
        {
#if DEBUG
            Console.WriteLine("UserAgent: " + LoginWebView.UserAgent);
#endif
            LoginWebView.UserAgent =
                LoginWebView.UserAgent
                .Replace("; wv)", ")")
                .Replace("; wv", ";");
            firstLoad = false;
        }

        var match = RedirectRegex.Match(e.Url);

        if (match.Success)
        {
            e.Cancel = true;

            string token = match.Groups[1].Value;
            Console.WriteLine(e.Url);
            Console.WriteLine(token);

            var result = await AuthService.Instance.GoogleLogin(token);

            // Show alert using the MainPage
            var message = result.Value ?? (result.Key == true ? null :
                                              result.Key == false ? (result.Value ?? LanguageService["UnknownError"].Current) :
                                              (result.Value ?? LanguageService["ServerError"].Current));
            if (message != null)
            {
                await this.DisplayAlert(LanguageService["GoogleLoginTitle"].Current, message, LanguageService["OK"].Current);
            }

            // Pop the login page
            if (Navigation.ModalStack.Contains(this))
                await Navigation.PopModalAsync();

            if (result.Key == true)
            {
                App.Current!.MainPage = new AppShell();
            }
        }
    }

    [GeneratedRegex(@"^https:\/\/mateszadam\.koyeb\.app\/([A-Za-z0-9\-]+)$", RegexOptions.Compiled)]
    private static partial Regex RedirectRegexCompiled();
}
