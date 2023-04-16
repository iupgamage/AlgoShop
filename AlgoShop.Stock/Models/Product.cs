using MongoDB.Bson.Serialization.Attributes;

namespace AlgoShop.Stock.Models
{
    public class Product
    {
        [BsonId]
        public int ID { get; set; }
        public int Quantity { get; set; }
    }
}
