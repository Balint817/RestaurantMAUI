using System.Text.RegularExpressions;
using CustomerApp.Services;

namespace CustomerApp.View;

public partial class GoogleLogin : ContentPage
{
    private static readonly Regex RedirectRegex = RedirectRegexCompiled();

    private const string FakeUserAgent = "Mozilla/5.0 (Linux; Android 4.1.1; Galaxy Nexus Build/JRO03C) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19";
    public GoogleLogin()
    {
        InitializeComponent();
        ClearWebViewData();
        LoginWebView.UserAgent = "fastFoodAuth";
        LoginWebView.Source = "https://mateszadam.koyeb.app/user/google";
    }
    private void ClearWebViewData()
    {
#if ANDROID
        var webView = new Android.Webkit.WebView(Android.App.Application.Context);
        Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
        Android.Webkit.CookieManager.Instance.Flush();
        webView.ClearCache(true);
        webView.ClearFormData();
        webView.ClearHistory();
#elif IOS
        var websiteDataTypes = new Foundation.NSSet<Foundation.NSString>(WebKit.WKWebsiteDataStore.AllWebsiteDataTypes);
        var dateFrom = Foundation.NSDate.DistantPast;
        WebKit.WKWebsiteDataStore.DefaultDataStore.RemoveData(websiteDataTypes, dateFrom, () => { });
#endif
    }

    private async void LoginWebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
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
                                              result.Key == false ? (result.Value ?? "Unknown error.") :
                                              (result.Value ?? "Internal server error. Try again later."));
            if (message != null)
            {
                await this.DisplayAlert("Google Login", message, "OK");
            }

            // Pop the login page
            if (Navigation.ModalStack.Contains(this))
                await Navigation.PopModalAsync();

            App.Current!.MainPage = new AppShell();
        }
    }

    [GeneratedRegex(@"^https:\/\/mateszadam\.koyeb\.app\/([A-Za-z0-9\-]+)$", RegexOptions.Compiled)]
    private static partial Regex RedirectRegexCompiled();
}
