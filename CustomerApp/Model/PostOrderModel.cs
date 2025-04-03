namespace CustomerApp.Model
{
#pragma warning disable
    public class PostOrderModel
    {
        public class Food
        {
            public int quantity { get; set; }
            public string _id { get; set; }
        }
        public string costumerId { get; set; }
        public List<Food> orderedProducts { get; set; }
        public static PostOrderModel FromCart(string costumer, IEnumerable<CartModel> cart)
        {
            return new()
            {
                costumerId = costumer,
                orderedProducts = cart.Select(x => new Food()
                {
                    quantity = x.Count,
                    _id = x.Food?._id
                }).ToList(),
            };
        }
    }
#pragma warning restore
}
