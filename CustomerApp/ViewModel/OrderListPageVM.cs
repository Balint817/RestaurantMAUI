using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.ViewModel
{
    public class OrderListPageVM
    {
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public Command BackCommand => AppShell.NavigateBackCommand;

        public AuthService.UserObject User => AuthService.Instance.User!;
        public ObservableCollection<OrderModel> Orders { get; set; } = new();

        bool loading;
        public OrderListPageVM()
        {
            Refresh();
        }
        public async Task Refresh()
        {
            if (loading)
            {
                return;
            }
            loading = true;
            try
            {
                var orders = await OrderService.Instance.GetOrders();
                Orders.Clear();
                foreach (var item in orders)
                {
                    Orders.Add(item);
                }
            }
            finally
            {
                loading = false;
            }
        }

        internal void OnDetailsTapped(Label sender)
        {
            if (sender.BindingContext is not OrderModel order)
            {
                return;
            }
            CartPage.ShowWindow(order);
        }
    }
}
