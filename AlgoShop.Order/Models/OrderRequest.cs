namespace AlgoShop.Order.Models
{
    public class OrderRequest
    {
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
