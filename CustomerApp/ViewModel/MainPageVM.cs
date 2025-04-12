using CustomerApp.Helpers;
using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.ViewModel
{
    public partial class MainPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;

        KeyValuePair<CategoryModel, FoodItemModel[]>[]? _array;
        public KeyValuePair<CategoryModel, FoodItemModel[]>[]? FirstThreeItemsOfCategories
        {
            get => _array;
            set
            {
                _array = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FirstAny));
                OnPropertyChanged(nameof(SecondAny));
            }
        }
        public KeyValuePair<CategoryModel, FoodItemModel[]>[]? ItemsByCategory;

        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; OnPropertyChanged(); }
        }

        public bool FirstAny => (FirstThreeItemsOfCategories?.FirstOrDefault().Value?.Length ?? 0) != 0;
        public bool SecondAny => (FirstThreeItemsOfCategories?.Skip(1).FirstOrDefault().Value?.Length ?? 0) != 0;

        private string? _searchEntry;
        public string? SearchEntry
        {
            get { return _searchEntry; }
            set
            {
                _searchEntry = value;
                OnPropertyChanged();
                Search();
            }
        }

        public MainPage Page { get; internal set; }
#pragma warning disable CS8618
        public MainPageVM()
#pragma warning restore CS8618
        {
            IsLoaded = false;
            CategoryService.Instance.GetAllAsTreeAsync()
                .ContinueWith(async categoriesTask =>
                {
                    try
                    {
                        var categoriesDict = (await categoriesTask).ToDictionary(x => x.Category._id, x => x.Category);
                        var foods = await FoodService.Instance.GetAllAsync();

                        //var g1 = foods.GroupBy(x => x.categoryId).ToArray();

                        //var g2 = g1.ToDictionary(x => categoriesDict[x.Key], x => x.ToArray()).ToArray();

                        ItemsByCategory = foods
                            .GroupBy(x => x.categoryId)
                            .ToDictionary(x => categoriesDict[x.Key], x => x.ToArray())
                            .ToArray();

                        SearchEntry = "";
                        IsLoaded = true;
                    }
                    catch (Exception)
                    {
                        await App.ShowGenericNetworkError();
                        ItemsByCategory = [];
                    }
                });
        }
        void Search()
        {
            FirstThreeItemsOfCategories = ItemsByCategory!
                .ToDictionary(x => x.Key, x => x.Value.Where(x => x.name.Trim().ToLower().Contains(SearchEntry?.Trim().ToLower() ?? "")).Take(3).ToArray())
                .ToArray();
        }

        internal async Task CategoryTapped(int idx)
        {
            if (!IsLoaded)
            {
                return;
            }
            var category = FirstThreeItemsOfCategories![idx].Key;
            await App.GetNavigation().PushAsync(new FoodListPage(category), true).MakeTaskBlocking(Page);
        }

        internal async Task CartTapped()
        {
            await CartPage.ShowWindow().MakeTaskBlocking(Page);
        }

        internal async Task UserTapped()
        {
            await App.GetNavigation().PushAsync(new OrderListPage(), true).MakeTaskBlocking(Page);
        }
    }
}
