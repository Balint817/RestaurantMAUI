using CustomerApp.Helpers;
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
        VM.Page = this;
    }

    internal static async Task ShowWindow()
    {
        var navigation = App.GetNavigation();
        OrderService.Instance.DisplayedCart = OrderService.Instance.Cart;
        await navigation.PushAsync(new CartPage(allowEdit: true), true);
    }
    internal static async Task ShowWindow(OrderModel orderModel)
    {
        var navigation = App.GetNavigation();
        OrderService.Instance.DisplayedCart = orderModel.ToCartModels();
        await navigation.PushAsync(new CartPage(allowEdit: false), true);
    }

    private async void EditTapped(object sender, TappedEventArgs e)
    {
        await VM.EditTapped((Image)sender);
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