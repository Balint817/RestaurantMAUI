using System.Text.Json.Serialization;

namespace CustomerApp.Model
{
    public class CategoryModel
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public string icon { get; set; }
    }
}
