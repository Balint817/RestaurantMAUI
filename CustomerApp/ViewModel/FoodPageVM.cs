using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Model;
using CustomerApp.Services;

namespace CustomerApp.ViewModel
{
    public class FoodPageVM : BindableObject
    {
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


        public FoodPageVM()
        {
            CartModel = new();
        }
        private bool _isEdit;

        public bool IsEditing
        {
            get { return _isEdit; }
            set { _isEdit = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNew)); }
        }
        public bool IsNew => !IsEditing;

        internal async void Init(FoodItemModel targetFood)
        {
            IsEditing = false;
            categories = (await CategoryService.Instance.GetAllAsync()).ToDictionary(x => x._id);
            materials = (await MaterialService.Instance.GetAllAsync()).ToDictionary(x => x._id);
            TargetFood = targetFood;
        }

        internal async void Init(CartModel model)
        {
            IsEditing = true;
            categories = (await CategoryService.Instance.GetAllAsync()).ToDictionary(x => x._id);
            materials = (await MaterialService.Instance.GetAllAsync()).ToDictionary(x => x._id);
            CartModel = model;
            TargetFood = model.Food; // to force UI updates
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
            await App.GetNavigation().PopAsync();
        }
    }
}
