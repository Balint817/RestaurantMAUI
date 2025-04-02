namespace CustomerApp.Model
{
    public class FoodItemModel
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public List<MaterialModel> materials { get; set; }
        public int price { get; set; }
        public string categoryId { get; set; }
        public List<string> subCategoryId { get; set; }
        public string image { get; set; }

        public string imageUrl => $"https://mateszadam.koyeb.app/images/name/{image}";
    }
}
