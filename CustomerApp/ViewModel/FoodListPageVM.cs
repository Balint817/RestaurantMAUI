using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CustomerApp.Resources.Styles;
using System.Text.Json.Serialization;
using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.View;
using CustomerApp.Helpers;

namespace CustomerApp.ViewModel
{
    public partial class FoodListPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        private FoodItemModel[]? _allFoodItems;


        private ObservableCollection<FoodItemModel>? _foodItems;
        public ObservableCollection<FoodItemModel>? FoodItems
        {
            get => _foodItems;
            set
            {
                _foodItems = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CategoryModel>? _subcategories;
        public ObservableCollection<CategoryModel>? Subcategories
        {
            get => _subcategories;
            set
            {
                _subcategories = value;
                OnPropertyChanged();
            }
        }

        private string? _searchEntry;
        public string? SearchEntry
        {
            get { return _searchEntry; }
            set { _searchEntry = value; }
        }


        public ICommand ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public ICommand BackCommand => AppShell.NavigateBackCommand;

#pragma warning disable CS8618
        public FoodListPageVM()
        {
            FoodItems = new ObservableCollection<FoodItemModel>();
            Subcategories = new ObservableCollection<CategoryModel>();
        }
#pragma warning restore CS8618

        private async Task LoadFoodItemsAsync()
        {
            var foodItems = await FoodService.Instance.GetAllAsync();
            _allFoodItems =
                foodItems.Where(f =>
                    f.categoryId == TargetMainCategory!._id
                    || f.subCategoryId
                    .Any(sc_id =>
                        _subcategories!
                        .Any(c => c._id == sc_id)))
                .ToArray();
        }

        private void Search(string? subcategoryId = null)
        {
            if (_allFoodItems is null)
            {
                return;
            }

            bool SearchPredicate(FoodItemModel food)
            {

                var categoryMatches =
                    subcategoryId is null
                    || food.categoryId == subcategoryId
                    || food.subCategoryId.Contains(subcategoryId);

                var nameMatches =
                    SearchEntry is null
                    || food.name.Trim().ToLower().Contains(SearchEntry.Trim().ToLower());

                return categoryMatches && nameMatches;
            }

            FoodItems = new(_allFoodItems.Where(SearchPredicate));
        }

        private async Task LoadSubcategoriesAsync()
        {
            var tree = await CategoryService.Instance.GetAllAsTreeAsync();
            var targetCategoryAsTree = tree.First(x => x.Category._id == TargetMainCategory!._id);
            var subcategories = targetCategoryAsTree.Flatten();
            Subcategories = new(subcategories);
        }

        Frame? selectedCategoryFrame;
        private CategoryModel? selectedSubcategory;

        public CategoryModel? SelectedSubcategory
        {
            get { return selectedSubcategory; }
            set { selectedSubcategory = value; OnPropertyChanged(); Search(SelectedSubcategory?._id); }
        }

        private CategoryModel? _targetMainCategory;

        public CategoryModel? TargetMainCategory
        {
            get { return _targetMainCategory; }
            set { _targetMainCategory = value; OnPropertyChanged(); }
        }

        public FoodListPage Page { get; internal set; }

        internal async void Init(CategoryModel category, FoodListPage page)
        {
            Page = page;
            TargetMainCategory = category;

            try
            {
                await LoadSubcategoriesAsync().ContinueWith(async t =>
                {
                    await LoadFoodItemsAsync();
                    Search();
                }).MakeTaskBlocking(page);
            }
            catch (Exception)
            {
                await App.ShowGenericNetworkError();
                Subcategories = [];
                FoodItems = [];
                Search();
            }
        }
        internal void OnSubCategoryTapped(object sender)
        {
            selectedCategoryFrame?.SetDynamicResource(Frame.BackgroundColorProperty, "ItemListBackground");

            var selectedCategoryLabel = (Label?)selectedCategoryFrame?.Content;
            selectedCategoryLabel?.SetDynamicResource(Label.TextColorProperty, "TextBrand");

            if (sender == selectedCategoryFrame)
            {
                SelectedSubcategory = null;
                selectedCategoryFrame = null;
                return;
            }

            var f = (Frame)sender;
            f.SetDynamicResource(Frame.BackgroundColorProperty, "TextBrand");

            selectedCategoryLabel = (Label)f.Content;
            selectedCategoryLabel.SetDynamicResource(Label.TextColorProperty, "ItemListBackground");

            selectedCategoryFrame = f;
            SelectedSubcategory = f.BindingContext as CategoryModel;
        }

        internal void OnImageTapped(object sender)
        {
            if (sender is not FoodItemModel f)
            {
                return;
            }
        }

        //internal void OnSearchTapped(object sender)
        //{
        //    //throw new NotImplementedException();
        //}

        internal async Task OnUserTapped(object sender)
        {
            await App.GetNavigation().PushAsync(new OrderListPage(), true).MakeTaskBlocking(Page);
        }

        internal async void CartTapped()
        {
            await CartPage.ShowWindow().MakeTaskBlocking(Page);
        }
    }
}