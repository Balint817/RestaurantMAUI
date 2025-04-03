using CustomerApp.Model;
using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class FoodListPage : ContentPage
{
    FoodListPageVM VM => (FoodListPageVM)this.BindingContext;
    public FoodListPage(CategoryModel category)
    {
        InitializeComponent();
        VM.Init(category);
    }

    private void OnCategoryTapped(object sender, TappedEventArgs e) => VM.OnSubCategoryTapped(sender);
    private void OnSearchTapped(object sender, TappedEventArgs e) => VM.OnSearchTapped(sender);
    private void OnUserTapped(object sender, TappedEventArgs e) => VM.OnUserTapped(sender);

    private void CartTapped(object sender, TappedEventArgs e)
    {
        VM.CartTapped();
    }
}