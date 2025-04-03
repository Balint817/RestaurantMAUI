using System.Text.Json.Serialization;

namespace CustomerApp.Model
{
    public class CategoryModel
    {
#pragma warning disable CS8618
        public string _id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string mainCategory { get; set; }
#pragma warning restore CS8618
    }
    public class CategoryTree
    {
        public CategoryModel Category { get; set; }
        public List<CategoryTree> Children { get; set; } = new();

        public IEnumerable<CategoryModel> Flatten()
        {
            return Children.SelectMany(child => child.FlattenInclusive());
        }
        public IEnumerable<CategoryModel> FlattenInclusive()
        {
            return Children.SelectMany(child => child.FlattenInclusive()).Prepend(Category);
        }
        public CategoryTree(CategoryModel category)
        {
            Category = category;
        }
    }
}
