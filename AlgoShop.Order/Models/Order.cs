using MongoDB.Bson.Serialization.Attributes;

namespace AlgoShop.Order.Models
{
    public class Order
    {
        [BsonId]
        public int ID { get; set; } 
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
