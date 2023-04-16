using AlgoShop.Order.Infrastructure;
using AlgoShop.Order.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using AlgoOrder = AlgoShop.Order.Models.Order;

namespace AlgoShop.Order.Services
{
    public interface IOrderService
    {
        Task<List<AlgoOrder>> GetAsync();
        Task<AlgoOrder> GetByIdAsync(int id);
        Task<int> CreateAsync(OrderRequest orderRequest);
        Task<bool> RemoveAsync();
        Task<bool> RemoveByIdAsync(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<AlgoOrder> _orderCollection;

        public OrderService(IOptions<AlgoShopDatabaseSettings> algoShopDatabaseSettings)
        {
            var mongoClient = new MongoClient(algoShopDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(algoShopDatabaseSettings.Value.DatabaseName);
            _orderCollection = mongoDatabase.GetCollection<AlgoOrder>(algoShopDatabaseSettings.Value.StockCollectionName);
        }

        public async Task<List<AlgoOrder>> GetAsync()
        {
            return await _orderCollection.Find(_ => true).ToListAsync();
        }

        public async Task<AlgoOrder> GetByIdAsync(int id)
        {
            return await _orderCollection.Find(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(OrderRequest orderRequest)
        {
            var order = new AlgoOrder
            {
                ID = new Random().Next(),
                Products = orderRequest.Products
            };

            await _orderCollection.InsertOneAsync(order);

            return order.ID;
        }

        public async Task<bool> RemoveAsync() 
        {
            var result = await _orderCollection.DeleteManyAsync(_ => true);

            return result.DeletedCount > 0;
        }

        public async Task<bool> RemoveByIdAsync(int id)
        {
            var result = await _orderCollection.DeleteOneAsync(x => x.ID == id);

            return result.DeletedCount > 0;
        }
    }
}
