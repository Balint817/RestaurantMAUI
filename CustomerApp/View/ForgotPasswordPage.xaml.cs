using CustomerApp.ViewModel;

namespace CustomerApp.View;

public partial class ForgotPasswordPage : ContentPage
{
    ForgotPasswordPageVM VM => (ForgotPasswordPageVM)this.BindingContext;
    public ForgotPasswordPage()
	{
		InitializeComponent();
        VM.Page = this;
    }
}