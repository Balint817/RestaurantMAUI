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
    public sealed class CategoryService: Singleton<CategoryService>
    {
        static CategoryService? _instance;
        public static CategoryService Instance => _instance ??= new();


        private CategoryModel[]? _items;
        static HttpService HttpService => HttpService.Instance;
        private CategoryService()
        {
            
        }
        public async Task<CategoryModel[]> GetAllAsync()
        {
            if (_items != null)
            {
                return _items;
            }

            var result = await HttpService.GetAllPagedItemsAsync<CategoryModel>($"{HttpService.BaseAPIUrl}/category", []);
            return _items = result;
        }
    }
}
