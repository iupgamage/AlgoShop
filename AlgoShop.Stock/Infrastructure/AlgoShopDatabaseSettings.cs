namespace AlgoShop.Stock.Infrastructure
{
    public class AlgoShopDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string StockCollectionName { get; set; } = null!; 
    }
}
