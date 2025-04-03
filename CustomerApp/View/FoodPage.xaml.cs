using CustomerApp.Model;
using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class FoodPage : ContentPage
{
    FoodPageVM VM => (FoodPageVM)BindingContext;
    public FoodPage(FoodItemModel model)
    {
        InitializeComponent();
        VM.Init(model);
    }
    public FoodPage(CartModel model)
    {
        InitializeComponent();
        VM.Init(model);
    }
    internal static void ShowWindow(FoodItemModel model)
    {
        App.GetNavigation().PushAsync(new FoodPage(model));
    }
    internal static void ShowWindow(CartModel model)
    {
        App.GetNavigation().PushAsync(new FoodPage(model));
    }

    private void PlusTapped(object sender, TappedEventArgs e) => VM.PlusTapped();

    private void MinusTapped(object sender, TappedEventArgs e) => VM.MinusTapped();

    private void AddItemTapped(object sender, TappedEventArgs e) => VM.AddItemTapped();
}