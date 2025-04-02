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
        }
        public static Command ToggleFlyoutCommand { get; }
        public AppShell()
        {
            InitializeComponent();

        }
    }
}
