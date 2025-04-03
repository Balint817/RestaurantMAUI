using System.Text.Json.Serialization;
using CustomerApp.Helpers;
using CustomerApp.Services;
using CustomerApp.View;

namespace CustomerApp.Model
{
    public class FoodItemModel : BindableObject
    {
#pragma warning disable CS8618
        public class Material
        {
            public string _id { get; set; }
            public int quantity { get; set; }
        }
        public string _id { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public List<Material> materials { get; set; }
        public int price { get; set; }
        public string categoryId { get; set; }
        public List<string> subCategoryId { get; set; }

        private string _image;
        public string image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(imageSource));
                OnPropertyChanged(nameof(imageUrl));
            }
        }
        [JsonIgnore]
        public string imageUrl => $"https://mateszadam.koyeb.app/images/name/{image}";

        [JsonIgnore]
        public ImageSource? imageSource => imageUrl.ToImageSourceFromUrl();
        [JsonIgnore]
        public Command PlusTappedCommand { get; }
        [JsonIgnore]
        public Command DetailCommand { get; }
        public FoodItemModel()
        {
            PlusTappedCommand = new Command(() =>
            {
                OnAction?.Invoke(this, FoodItemAction.Plus);
            });
            DetailCommand = new Command(() =>
            {
                OnAction?.Invoke(this, FoodItemAction.Detail);
            });
        }

        public event Action<FoodItemModel, FoodItemAction>? OnAction;
#pragma warning restore CS8618

        public static void GenericOnFoodAction(FoodItemModel model, FoodItemAction action)
        {
            switch (action)
            {
                case FoodItemAction.Plus:
                    OrderService.Instance.AddToCart(model, 1);
                    break;
                case FoodItemAction.Detail:
                    FoodPage.ShowWindow(model);
                    break;
                default:
                    break;
            }
        }
    }

    public enum FoodItemAction
    {
        Plus,
        Detail
    }
}
