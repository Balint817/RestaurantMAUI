using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Model
{
#pragma warning disable CS8618
    public class OrderModel
    {
        public class Product
        {
            public int quantity { get; set; }
            public FoodItemModel details { get; set; }
        }
        public string _id { get; set; }
        public string costumerId { get; set; }
        public DateTime orderedTime { get; set; }
        public int totalPrice { get; set; }
        public object finishedCokingTime { get; set; }
        public object finishedTime { get; set; }
        public List<Product> orderedProducts { get; set; }
        public int orderNumber { get; set; }

        public List<CartModel> ToCartModels()
        {
            return orderedProducts.Select(x => new CartModel() { Count = x.quantity, Food = x.details }).ToList();
        }
    }
#pragma warning restore CS8618
}
