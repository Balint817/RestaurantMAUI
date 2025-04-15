using System.Text.RegularExpressions;
using CustomerApp.Services;

namespace CustomerApp.View;

public partial class GoogleLogin : ContentPage
{
    private static readonly Regex RedirectRegex = RedirectRegexCompiled();

    public GoogleLogin()
    {
        //InitializeComponent();
        //LoginWebView.Source = "https://mateszadam.koyeb.app/user/google";
    }

    private async void LoginWebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        var match = RedirectRegex.Match(e.Url);

        if (match.Success)
        {
            e.Cancel = true;

            string token = match.Groups[1].Value;

            var result = await AuthService.Instance.GoogleLogin(token);

            // Show alert using the MainPage
            var message = result.Value ?? (result.Key == true ? null :
                                              result.Key == false ? (result.Value ?? "Unknown error.") :
                                              (result.Value ?? "Internal server error. Try again later."));

            await this.DisplayAlert("Google Login", message, "OK");

            // Pop the login page
            if (Navigation.NavigationStack.Contains(this))
                await Navigation.PopAsync();
        }
    }

    [GeneratedRegex(@"^https:\/\/mateszadam\.koyeb\.app\/([\w\-]+)$", RegexOptions.Compiled)]
    private static partial Regex RedirectRegexCompiled();
}
