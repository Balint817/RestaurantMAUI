using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Helpers;
using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.ViewModel
{
    public partial class CartPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;

        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public Command BackCommand => AppShell.NavigateBackCommand;
#pragma warning disable CS8618
        public CartPageVM()
#pragma warning restore CS8618
        {

        }
        public List<CartModel>? Items => OrderService.Instance.DisplayedCart?.ToList();

        private bool _allowEdit;

        public bool AllowEdit
        {
            get { return _allowEdit; }
            set { _allowEdit = value; OnPropertyChanged(); }
        }

        public CartPage Page { get; internal set; }

        internal async Task EditTapped(Image sender)
        {
            if (!AllowEdit)
            {
                return;
            }
            var cartModel = (CartModel)sender.BindingContext;
            await FoodPage.ShowWindow(cartModel).MakeTaskBlocking(Page);
        }

        internal void CancelTapped(Image sender)
        {
            if (!AllowEdit)
            {
                return;
            }
            var cartModel = (CartModel)sender.BindingContext;
            OrderService.Instance.DisplayedCart!.Remove(cartModel);
            OnPropertyChanged(nameof(Items));
        }

        internal async void CheckoutTapped()
        {
            if (!AllowEdit)
            {
                return;
            }
            var orderService = OrderService.Instance;
            orderService.Cart = orderService.DisplayedCart!;
            if (orderService.Cart.Count == 0)
            {
                await Page.DisplayAlert(LanguageService["ErrorTitle"].Current, LanguageService["EmptyCartError"].Current, LanguageService["OK"].Current);
                return;
            }
            try
            {
                await orderService.PlaceOrder().MakeTaskBlocking(Page);
                await Page.DisplayAlert(LanguageService["Success"].Current, LanguageService["PlaceOrderSuccess"].Current, LanguageService["OK"].Current);
                orderService.Cart.Clear();
                BackCommand.Execute(null);
            }
            catch (Exception ex)
            {
                await Page.DisplayAlert(LanguageService["ErrorTitle"].Current, LanguageService["PlaceOrderError"].Current, LanguageService["OK"].Current);
            }
            finally
            {

            }
        }
    }
}
