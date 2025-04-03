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
        private CategoryTree[]? _itemsTree;
        static HttpService HttpService => HttpService.Instance;
        private CategoryService()
        {
            
        }
        public async Task<CategoryModel[]> GetAllAsync()
        {
            if (_items != null)
            {
                return [.. _items];
            }

            var result = await HttpService.GetAllPagedItemsAsync<CategoryModel>($"{HttpService.BaseAPIUrl}/category", []);
            return _items = result;
        }

        public async Task<CategoryTree[]> GetAllAsTreeAsync()
        {
            if (_itemsTree != null)
            {
                return [.. _itemsTree];
            }
            return _itemsTree = [.. ToTree(await GetAllAsync())];
        }

        public static List<CategoryTree> ToTree(IEnumerable<CategoryModel> categories)
        {
            var categoryDictionary = categories.ToDictionary(c => c._id, c => new CategoryTree(c));
            var rootNodes = new List<CategoryTree>();

            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.mainCategory) || !categoryDictionary.TryGetValue(category.mainCategory, out var mainCategory))
                {
                    rootNodes.Add(categoryDictionary[category._id]);
                }
                else
                {
                    mainCategory.Children.Add(categoryDictionary[category._id]);
                }
            }

            return rootNodes;
        }
    }
}
