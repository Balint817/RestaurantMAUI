using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Helpers;
using CustomerApp.Model;
using CustomerApp.Services;
using CustomerApp.View;
using static Android.Graphics.ColorSpace;

namespace CustomerApp.ViewModel
{
    public partial class FoodPageVM : BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public Command BackCommand => AppShell.NavigateBackCommand;

        private CartModel _cartModel;
        public CartModel CartModel
        {
            get { return _cartModel; }
            [MemberNotNull(nameof(_cartModel))]
            set { _cartModel = value; }
        }

        private FoodItemModel? _targetFood;

        public FoodItemModel? TargetFood
        {
            get { return _targetFood; }
            set
            {
                CartModel.Food = _targetFood = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TargetFoodCategory));
                OnPropertyChanged(nameof(CartModel));
                OnPropertyChanged(nameof(CountedMaterials));
            }
        }

        Dictionary<string, CategoryModel>? categories;

        Dictionary<string, MaterialModel>? materials;
        public CategoryModel? TargetFoodCategory
        {
            get
            {
                if (TargetFood is null)
                {
                    return null;
                }
                CategoryModel? c;
                foreach (var item in TargetFood.subCategoryId)
                {
                    if (categories!.TryGetValue(item, out c))
                    {
                        return c;
                    }
                }
                if (categories!.TryGetValue(TargetFood.categoryId, out c))
                {
                    return c;
                }
                return null;
            }
        }
        public IEnumerable<MaterialModel.Quantized> CountedMaterials
        {
            get
            {
                if (TargetFood is null)
                {
                    yield break;
                }

                foreach (var x in TargetFood.materials)
                {

                    if (!materials!.TryGetValue(x._id, out var material))
                    {
                        yield return MaterialModel.Quantized.Unknown;
                        continue;
                    }
                    yield return material.AddCount(x.quantity);
                }

            }
        }


#pragma warning disable CS8618
        public FoodPageVM()
        {
            CartModel = new();
        }
#pragma warning restore CS8618
        private bool _isEdit;

        public bool IsEditing
        {
            get { return _isEdit; }
            set { _isEdit = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNew)); }
        }
        public bool IsNew => !IsEditing;

        FoodPage _page;

        private async Task BaseInit(FoodPage page, FoodItemModel targetFood)
        {
            try
            {
                categories = (await CategoryService.Instance.GetAllAsync().MakeTaskBlocking(page)).ToDictionary(x => x._id);
                materials = (await MaterialService.Instance.GetAllAsync().MakeTaskBlocking(page)).ToDictionary(x => x._id);
            }
            catch (Exception)
            {
                await App.ShowGenericNetworkError();
                categories = [];
                materials = [];
            }
            TargetFood = targetFood; // to force UI update
        }
        internal async void Init(FoodPage page, FoodItemModel targetFood)
        {
            _page = page;
            IsEditing = false;
            await BaseInit(page, targetFood);
        }

        internal async void Init(FoodPage page, CartModel model)
        {
            CartModel = model;
            IsEditing = true;
            await BaseInit(page, model.Food!);
        }

        internal void PlusTapped()
        {
            CartModel.Count++;
        }

        internal void MinusTapped()
        {
            CartModel.Count--;
        }

        internal async void AddItemTapped()
        {
            if (!IsEditing)
            {
                OrderService.Instance.Cart.Add(CartModel);
            }
            await App.GetNavigation().PopAsync().MakeTaskBlocking(_page);
        }
    }
}
