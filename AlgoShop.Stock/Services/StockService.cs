using AlgoShop.Stock.Infrastructure;
using AlgoShop.Stock.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlgoShop.Stock.Services
{
    public interface IStockService
    {
        Task<List<Product>> GetAsync();
        Task<bool> CreateAsync(Product product);
        Task<bool> IncreaseAsync(Product product);
        Task<bool> DecreaseAsync(Product product);
        Task<bool> ReserveAsync(Reservation reservation);
        Task<bool> RemoveAsync();
        Task<bool> RemoveByIdAsync(int id);
    }

    public class StockService : IStockService
    {
        private readonly IMongoCollection<Product> _productCollection;

        public StockService(IOptions<AlgoShopDatabaseSettings> algoShopDatabaseSettings)
        {
            var mongoClient = new MongoClient(algoShopDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(algoShopDatabaseSettings.Value.DatabaseName);
            _productCollection = mongoDatabase.GetCollection<Product>(algoShopDatabaseSettings.Value.StockCollectionName);
        }

        public async Task<List<Product>> GetAsync()
        {
            return await _productCollection.Find(_ => true).ToListAsync();
        }

        public async Task<bool> CreateAsync(Product product)
        {
            var products = await _productCollection.FindAsync(x => x.ID == product.ID);

            if (products.Any())
            {
                throw new ArgumentException("Product already exists");
            }

            await _productCollection.InsertOneAsync(product);

            return true;
        }

        public async Task<bool> IncreaseAsync(Product product)
        {
            var products = await _productCollection.FindAsync(x => x.ID == product.ID);

            if (!products.Any())
            {
                throw new ArgumentException("Product not found");
            }

            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq("ID", product.ID);
            UpdateDefinition<Product> update = Builders<Product>.Update.Inc("Quantity", product.Quantity);

            await _productCollection.UpdateOneAsync(filter, update);

            return true;
        }

        public async Task<bool> DecreaseAsync(Product product)
        {
            var products = await _productCollection.Find(x => x.ID == product.ID).ToListAsync();

            if (!products.Any())
            {
                throw new ArgumentException("Product not found");
            }

            var productInStock = products.Single();

            if (productInStock.Quantity < product.Quantity)
            {
                throw new Exception("Insufficient stock");
            }

            var quantityLeft = productInStock.Quantity - product.Quantity;

            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq("ID", productInStock.ID);
            UpdateDefinition<Product> update = Builders<Product>.Update.Set("Quantity", quantityLeft);

            await _productCollection.UpdateOneAsync(filter, update);

            return true;
        }

        public async Task<bool> ReserveAsync(Reservation reservation)
        {
            var productIds = reservation.Products.Select(x => x.ID);

            FilterDefinition<Product> productsFilter = Builders<Product>.Filter.In("ID", productIds);

            var productsInStock = await _productCollection.Find(productsFilter).ToListAsync();

            var nonExistentProducts = productIds.Except(productsInStock.Select(x => x.ID));

            if (nonExistentProducts.Any())
            {
                throw new Exception($"Stock not found for the products: {string.Join(",", nonExistentProducts)}");
            }

            Dictionary<int, int> Products = new Dictionary<int, int>();

            foreach (var product in reservation.Products)
            {
                var productInStock = productsInStock.Single(x => x.ID == product.ID);

                if (productInStock.Quantity < product.Quantity)
                {
                    throw new Exception($"Insufficient stock for the product {product.ID}");
                }

                Products.Add(product.ID, productInStock.Quantity - product.Quantity);
            }

            foreach (var product in Products)
            {
                FilterDefinition<Product> filter = Builders<Product>.Filter.Eq("ID", product.Key);
                UpdateDefinition<Product> update = Builders<Product>.Update.Set("Quantity", product.Value);

                await _productCollection.UpdateOneAsync(filter, update);
            }

            return true;
        }

        public async Task<bool> RemoveAsync()
        {
            var result = await _productCollection.DeleteManyAsync(_ => true);

            return result.DeletedCount > 0;
        }

        public async Task<bool> RemoveByIdAsync(int id)
        {
            var result = await _productCollection.DeleteOneAsync(x => x.ID == id);

            return result.DeletedCount > 0;
        }
    }
}
