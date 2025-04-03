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
    public class MainPageVM: BindableObject
    {
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
            set { _searchEntry = value;
                OnPropertyChanged();
                Search(); }
        }

        public MainPageVM()
        {
            IsLoaded = false;
            CategoryService.Instance.GetAllAsTreeAsync()
                .ContinueWith(categoriesTask =>
                {
                    FoodService.Instance.GetAllAsync()
                        .ContinueWith(foodsTask =>
                        {
                            var categoriesDict = categoriesTask.Result.ToDictionary(x => x.Category._id, x => x.Category);
                            var foods = foodsTask.Result;

                            //var g1 = foods.GroupBy(x => x.categoryId).ToArray();

                            //var g2 = g1.ToDictionary(x => categoriesDict[x.Key], x => x.ToArray()).ToArray();

                            ItemsByCategory = foods
                                .GroupBy(x => x.categoryId)
                                .ToDictionary(x => categoriesDict[x.Key], x => x.ToArray())
                                .ToArray();

                            foreach (var item in foods)
                            {
                                item.OnAction += FoodItemModel.GenericOnFoodAction;
                            }
                            SearchEntry = "";
                            IsLoaded = true;
                        });
                });
        }
        void Search()
        {
            FirstThreeItemsOfCategories = ItemsByCategory!
                .ToDictionary(x => x.Key, x => x.Value.Where(x => x.name.Trim().ToLower().Contains(SearchEntry?.Trim().ToLower() ?? "")).Take(3).ToArray())
                .ToArray();
        }

        internal async void CategoryTapped(int idx)
        {
            var category = FirstThreeItemsOfCategories![idx].Key;
            await Application.Current!.MainPage!.Navigation.PushAsync(new FoodListPage(category));
        }

        internal void CartTapped()
        {
            CartPage.ShowWindow();
        }

        internal void UserTapped()
        {
            App.GetNavigation().PushAsync(new OrderListPage());
        }
    }
}
