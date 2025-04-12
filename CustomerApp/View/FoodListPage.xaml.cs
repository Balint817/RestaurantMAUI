using CustomerApp.Helpers;
using CustomerApp.Model;
using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class FoodListPage : ContentPage
{
    FoodListPageVM VM => (FoodListPageVM)this.BindingContext;
    public FoodListPage(CategoryModel category)
    {
        InitializeComponent();
        VM.Init(category, this);
    }

    //private void OnSearchTapped(object sender, TappedEventArgs e) => VM.OnSearchTapped(sender);
    private void OnCategoryTapped(object sender, TappedEventArgs e) => VM.OnSubCategoryTapped(sender);
    private async void OnUserTapped(object sender, TappedEventArgs e) => await VM.OnUserTapped(sender);

    private void CartTapped(object sender, TappedEventArgs e)
    {
        VM.CartTapped();
    }

    private async void Food_PlusTapped(object sender, TappedEventArgs e)
    {
        if (sender is not Label l)
            return;
        if (l.BindingContext is not FoodItemModel model)
            return;
        await FoodItemModel.GenericOnFoodAction(model, FoodItemAction.Detail).MakeTaskBlocking(this);
    }

    private async void Food_DetailTapped(object sender, TappedEventArgs e)
    {
        if (sender is not Image img)
            return;
        if (img.BindingContext is not FoodItemModel model)
            return;
        await FoodItemModel.GenericOnFoodAction(model, FoodItemAction.Detail).MakeTaskBlocking(this);
    }
}