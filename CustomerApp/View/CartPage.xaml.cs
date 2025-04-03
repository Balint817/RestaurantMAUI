using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class CartPage : ContentPage
{
    CartPageVM VM => (CartPageVM)BindingContext;
    public CartPage(bool allowEdit = true)
    {
        InitializeComponent();
        VM.AllowEdit = allowEdit;
    }

    internal static async void ShowWindow()
    {
        var navigation = App.GetNavigation();
        OrderService.Instance.DisplayedCart = OrderService.Instance.Cart;
        await navigation.PushAsync(new CartPage(allowEdit: true));
    }
    internal static async void ShowWindow(OrderModel orderModel)
    {
        var navigation = App.GetNavigation();
        OrderService.Instance.DisplayedCart = orderModel.ToCartModels();
        await navigation.PushAsync(new CartPage(allowEdit: false));
    }

    private void EditTapped(object sender, TappedEventArgs e)
    {
        if (!VM.AllowEdit)
        {
            DisplayAlert("Nincs engedély", "Nem tud egy már leadott rendelést szerkeszteni!", "OK");
            return;
        }
        VM.EditTapped((Image)sender);
    }

    // protected override void OnDisappearing()
    // {
    //     base.OnDisappearing();
    //     if (VM.AllowEdit)
    //     {
    ////OrderService.Instance.Cart = OrderService.Instance.DisplayedCart;
    //     }
    // }

    private void CancelTapped(object sender, TappedEventArgs e) => VM.CancelTapped((Image)sender);

    private void CheckoutTapped(object sender, TappedEventArgs e) => VM.CheckoutTapped();
}