using CustomerApp.Helpers;
using CustomerApp.Model;
using CustomerApp.ViewModel;
using Microsoft.Maui.Controls.Shapes;

namespace CustomerApp.View
{
    public partial class MainPage : ContentPage
    {
        MainPageVM VM => (MainPageVM)BindingContext;
        public MainPage()
        {
            InitializeComponent();
            VM.Page = this;
        }

        private async void FirstCategory_Tapped(object sender, TappedEventArgs e)
        {
            await VM.CategoryTapped(0);
        }

        private async void SecondCategory_Tapped(object sender, TappedEventArgs e)
        {
            await VM.CategoryTapped(1);
        }

        private async void CartTapped(object sender, TappedEventArgs e)
        {
            await VM.CartTapped();
        }

        private async void UserTapped(object sender, TappedEventArgs e)
        {
            await VM.UserTapped();
        }

        private async void Food_PlusTapped(object sender, TappedEventArgs e)
        {
            if (sender is not Grid g)
                return;
            if (g.BindingContext is not FoodItemModel model)
                return;
            await FoodItemModel.GenericOnFoodAction(model, FoodItemAction.Plus).MakeTaskBlocking(this);
        }

        private async void Food_DetailTapped(object sender, TappedEventArgs e)
        {
            if (sender is not Grid g)
                return;
            if (g.BindingContext is not FoodItemModel model)
                return;
            await FoodItemModel.GenericOnFoodAction(model, FoodItemAction.Detail).MakeTaskBlocking(this);
        }
    }

}
