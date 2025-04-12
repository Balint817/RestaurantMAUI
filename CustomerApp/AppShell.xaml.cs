using CustomerApp.View;

namespace CustomerApp
{
    public partial class AppShell : Shell
    {
        static AppShell()
        {
            ToggleFlyoutCommand = new Command(() =>
            {
                Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
            });
            NavigateBackCommand = new Command(async () =>
            {
                await NavigateBack();
            });
        }
        public static Command ToggleFlyoutCommand { get; }
        public static Command NavigateBackCommand { get; }
        public static async Task NavigateBack()
        {
            var navigation = App.GetNavigation();
            if (navigation.NavigationStack.Count != 0)
            {
                await navigation.PopAsync(true);
                return;
            }
            if (AppShell.Current.CurrentPage is not MainPage)
            {
                await AppShell.Current.GoToAsync("//MainPage", true);
            }
        }
        public AppShell()
        {
            InitializeComponent();

        }
    }
}
