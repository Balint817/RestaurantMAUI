using CustomerApp.Model;
using CustomerApp.Services;
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

        KeyValuePair<CategoryModel, FoodItemModel[]>[] _array;
        public KeyValuePair<CategoryModel, FoodItemModel[]>[] FirstThreeItemsOfCategories
        {
            get => _array;
            set
            {
                _array = value;
                OnPropertyChanged();
            }
        }
        public MainPageVM()
        {
            CategoryService.Instance.GetAllAsync()
                .ContinueWith(categoriesTask =>
                {
                    FoodService.Instance.GetAllAsync()
                        .ContinueWith(foodsTask =>
                        {
                            var categories = categoriesTask.Result.ToDictionary(x => x._id, x => x);
                            var foods = foodsTask.Result;

                            FirstThreeItemsOfCategories = foods
                                .GroupBy(x => x.categoryId)
                                .Select(x => new KeyValuePair<CategoryModel, FoodItemModel[]>(
                                    categories[x.Key],
                                    x.Take(3).ToArray()))
                                .ToArray();
                        });
                });
        }
    }
}
