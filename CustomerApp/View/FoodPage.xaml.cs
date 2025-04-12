using CustomerApp.Model;
using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class FoodPage : ContentPage
{
    FoodPageVM VM => (FoodPageVM)BindingContext;
    public FoodPage(FoodItemModel model)
    {
        InitializeComponent();
        VM.Init(this, model);
    }
    public FoodPage(CartModel model)
    {
        InitializeComponent();
        VM.Init(this, model);
    }
    internal static async Task ShowWindow(FoodItemModel model)
    {
        await App.GetNavigation().PushAsync(new FoodPage(model), true);
    }
    internal static async Task ShowWindow(CartModel model)
    {
        await App.GetNavigation().PushAsync(new FoodPage(model), true);
    }

    private void PlusTapped(object sender, TappedEventArgs e) => VM.PlusTapped();

    private void MinusTapped(object sender, TappedEventArgs e) => VM.MinusTapped();

    private void AddItemTapped(object sender, TappedEventArgs e) => VM.AddItemTapped();
}