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

namespace CustomerApp.ViewModel
{
    public class FoodListPageVM : BindableObject
    {
        private ObservableCollection<FoodItemModel> _foodItems;
        private ObservableCollection<CategoryModel> _categories;

        public ObservableCollection<FoodItemModel> FoodItems
        {
            get => _foodItems;
            set
            {
                _foodItems = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoryModel> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;

        public FoodListPageVM()
        {
            FoodItems = new ObservableCollection<FoodItemModel>();
            Categories = new ObservableCollection<CategoryModel>();
            LoadCategoriesAsync();
            LoadFoodItemsAsync();
        }

        private async Task LoadFoodItemsAsync(string? categoryId = null)
        {
            FoodItems = new((await FoodService.Instance.GetAllAsync())
                .Where(x => categoryId is null || x.categoryId == categoryId));
        }

        private async Task LoadCategoriesAsync()
        {
            Categories = new(await CategoryService.Instance.GetAllAsync());
        }

        Frame? selectedCategoryFrame;
        private CategoryModel? selectedCategory;

        public CategoryModel? SelectedCategory
        {
            get { return selectedCategory; }
            set { selectedCategory = value; OnPropertyChanged(); }
        }

        internal void OnCategoryTapped(object sender)
        {
            selectedCategoryFrame?.SetDynamicResource(Frame.BackgroundColorProperty, "ItemListBackground");

            var selectedCategoryLabel = (Label?)selectedCategoryFrame?.Content;
            selectedCategoryLabel?.SetDynamicResource(Label.TextColorProperty, "TextBrand");

            if (sender == selectedCategoryFrame)
            {
                selectedCategory = null;
                selectedCategoryFrame = null;
                return;
            }

            var f = (Frame)sender;
            f.SetDynamicResource(Frame.BackgroundColorProperty, "TextBrand");

            selectedCategoryLabel = (Label)f.Content;
            selectedCategoryLabel.SetDynamicResource(Label.TextColorProperty, "ItemListBackground");

            selectedCategoryFrame = f;
            SelectedCategory = f.BindingContext as CategoryModel;
        }

        internal void OnPlusTapped(object sender)
        {
            throw new NotImplementedException();
        }

        internal void OnImageTapped(object sender)
        {
            throw new NotImplementedException();
        }

        internal void OnSearchTapped(object sender)
        {
            throw new NotImplementedException();
        }

        internal void OnUserTapped(object sender)
        {
            throw new NotImplementedException();
        }
    }
}