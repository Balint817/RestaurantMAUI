namespace CustomerApp.Model
{
    public class CartModel : BindableObject
    {
        private FoodItemModel? _food;

        public FoodItemModel? Food
        {
            get { return _food; }
            set { _food = value; OnPropertyChanged(); }
        }

        private int _count = 1;
        public int Count
        {
            get { return _count; }
            set { _count = Math.Max(1, value); OnPropertyChanged(); }
        }
    }
}
