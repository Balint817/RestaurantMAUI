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
    public sealed class FoodService : Singleton<FoodService>
    {
        static FoodService? _instance;
        public static FoodService Instance => _instance ??= new();


        private FoodItemModel[]? _items;
        static HttpService HttpService => HttpService.Instance;
        private FoodService()
        {

        }
        public async Task<FoodItemModel[]> GetAllAsync()
        {
            if (_items != null)
            {
                return _items;
            }

            var result = await HttpService.GetAllPagedItemsAsync<FoodItemModel>($"{HttpService.BaseAPIUrl}/food", []);
            var l = result.Length;
            return _items = result;
        }
    }
}
