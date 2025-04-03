using CustomerApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomerApp.Services
{
    public sealed class OrderService : Singleton<OrderService>
    {
        static OrderService? _instance;
        public static OrderService Instance => _instance ??= new();
        static HttpService HttpService => HttpService.Instance;
        public List<CartModel>? DisplayedCart { get; set; }
        public List<CartModel> Cart { get; set; } = new();

        public void AddToCart(FoodItemModel model, int count = 1)
        {
            Cart.Add(new() { Food = model, Count = count });
        }
        private OrderService()
        {

        }
        public async Task<OrderModel[]> GetOrders()
        {
            var queryParams = new Dictionary<string, object?>()
            {
                ["costumerId"] = AuthService.Instance.User!.userId
            };
            return await HttpService.GetAllPagedItemsAsync<OrderModel>($"{HttpService.BaseAPIUrl}/order", queryParams);
        }

        public async Task PlaceOrder()
        {
            var user = AuthService.Instance.User;
            var userId = user!.userId;
            var order = PostOrderModel.FromCart(userId, Cart);
            var response = await HttpService.PostJsonAsync($"{HttpService.BaseAPIUrl}/order", order);

        }
    }
}
