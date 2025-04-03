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
    public sealed class MaterialService : Singleton<MaterialService>
    {
        static MaterialService? _instance;
        public static MaterialService Instance => _instance ??= new();


        private MaterialModel[]? _items;
        static HttpService HttpService => HttpService.Instance;
        private MaterialService()
        {

        }
        public async Task<MaterialModel[]> GetAllAsync()
        {
            if (_items != null)
            {
                return _items;
            }

            var result = await HttpService.GetAllPagedItemsAsync<MaterialModel>($"{HttpService.BaseAPIUrl}/material", []);
            var l = result.Length;
            return _items = result;
        }
    }
}
