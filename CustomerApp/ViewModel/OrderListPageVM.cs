﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CustomerApp.Helpers;
using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.ViewModel
{
    public partial class OrderListPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }
        //public Command RefreshCommand => new Command(OnRefresh);

        //private async void OnRefresh()
        //{
        //    await Refresh();
        //}
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public Command BackCommand => AppShell.NavigateBackCommand;

        public AuthService.UserObject User => AuthService.Instance.User!;
        public ObservableCollection<OrderModel> Orders { get; set; } = new();
        public OrderListPageVM()
        {
            Refresh();
        }
        public async Task Refresh()
        {
            if (IsRefreshing)
            {
                return;
            }
            IsRefreshing = true;
            try
            {
                var orders = await OrderService.Instance.GetOrders();
                Orders.Clear();
                foreach (var item in orders)
                {
                    Orders.Add(item);
                }
            }
            catch
            {
                Orders.Clear();
                await App.ShowGenericNetworkError();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        internal async Task OnDetailsTapped(BindableObject sender)
        {
            if (sender.BindingContext is not OrderModel order)
            {
                return;
            }
            await CartPage.ShowWindow(order);
        }

        internal async Task OnLogout()
        {
            if (IsRefreshing)
            {
                return;
            }
            IsRefreshing = true;
            try
            {
                await AuthService.Instance.Logout();
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
