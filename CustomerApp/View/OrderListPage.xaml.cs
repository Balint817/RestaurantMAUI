using CustomerApp.Helpers;
using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class OrderListPage : ContentPage
{
    OrderListPageVM VM => (OrderListPageVM)BindingContext;
    public OrderListPage()
    {
        InitializeComponent();
    }

    private async void OnDetailsTapped(object sender, TappedEventArgs e) => await VM.OnDetailsTapped((BindableObject)sender).MakeTaskBlocking(this);

    private async void OnLogoutTapped(object sender, TappedEventArgs e) => await VM.OnLogout().MakeTaskBlocking(this);

    private async void BackTapped(object sender, TappedEventArgs e)
    {
        await AppShell.NavigateBack().MakeTaskBlocking(this);
    }
}