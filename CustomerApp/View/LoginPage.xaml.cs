using CustomerApp.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CustomerApp.ViewModel;

namespace CustomerApp.View
{
    public partial class LoginPage : ContentPage
    {
        LoginPageVM VM => (LoginPageVM)this.BindingContext;
        public LoginPage()
        {
            InitializeComponent();
            VM.Page = this;
        }
    }
}
