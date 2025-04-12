using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class RegisterPage : ContentPage
{
    RegisterPageVM VM => (RegisterPageVM)this.BindingContext;
    public RegisterPage()
    {
        InitializeComponent();
        VM.Page = this;
    }
}