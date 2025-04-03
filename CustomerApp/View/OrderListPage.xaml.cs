using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class OrderListPage : ContentPage
{
    OrderListPageVM VM => (OrderListPageVM)BindingContext;
    public OrderListPage()
    {
        InitializeComponent();
    }

    private void OnDetailsTapped(object sender, TappedEventArgs e) => VM.OnDetailsTapped((Label)sender);

    private void OnLogoutTapped(object sender, TappedEventArgs e) => VM.OnLogout();

    private async void BackTapped(object sender, TappedEventArgs e)
    {
        await AppShell.NavigateBack();
    }
}