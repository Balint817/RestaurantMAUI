using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.ViewModel
{
    public class CartPageVM: BindableObject
    {

        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public Command BackCommand => AppShell.NavigateBackCommand;
        public CartPageVM()
        {

        }
        public List<CartModel>? Items =>  OrderService.Instance.DisplayedCart;

        private bool _allowEdit;

        public bool AllowEdit
        {
            get { return _allowEdit; }
            set { _allowEdit = value; OnPropertyChanged(); }
        }

        internal void EditTapped(Image sender)
        {
            var cartModel = (CartModel)sender.BindingContext;
            FoodPage.ShowWindow(cartModel);
        }

        internal void CancelTapped(Image sender)
        {
            var cartModel = (CartModel)sender.BindingContext;
            OrderService.Instance.DisplayedCart!.Remove(cartModel);
        }

        internal async void CheckoutTapped()
        {
            if (!AllowEdit)
            {
                return;
            }
            var orderService = OrderService.Instance;
            orderService.Cart = orderService.DisplayedCart!;
            await orderService.PlaceOrder();
            BackCommand.Execute(null);
        }
    }
}
