using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class FoodListPage : ContentPage
{
	FoodListPageVM VM => (FoodListPageVM)this.BindingContext;
	public FoodListPage()
	{
		InitializeComponent();


    }

    private void OnCategoryTapped(object sender, TappedEventArgs e) => VM.OnCategoryTapped(sender);
    private void OnPlusTapped(object sender, TappedEventArgs e) => VM.OnPlusTapped(sender);
    private void OnImageTapped(object sender, TappedEventArgs e) => VM.OnImageTapped(sender);
    private void OnSearchTapped(object sender, TappedEventArgs e) => VM.OnSearchTapped(sender);
    private void OnUserTapped(object sender, TappedEventArgs e) => VM.OnUserTapped(sender);
}