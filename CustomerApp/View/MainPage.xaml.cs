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
        }

        private void FirstCategory_Tapped(object sender, TappedEventArgs e)
        {
            VM.CategoryTapped(0);
        }

        private void SecondCategory_Tapped(object sender, TappedEventArgs e)
        {
            VM.CategoryTapped(1);
        }

        private void CartTapped(object sender, TappedEventArgs e)
        {
            VM.CartTapped();
        }

        private void UserTapped(object sender, TappedEventArgs e)
        {
            VM.UserTapped();
        }
    }

}
