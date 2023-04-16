using AlgoShop.Order.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using AlgoOrder = AlgoShop.Order.Models.Order;

namespace AlgoShop.Test.AlgoShopOrderTests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private readonly HttpClient _httpClient;

        public OrderControllerTests()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [Test]
        public async Task Create_and_delete_an_order()
        {
            var orderRequest = new OrderRequest
            {
                Products = new List<Product>()
                {
                    new Product
                    {
                        ID = 2,
                        Quantity = 5,
                    },
                    new Product
                    {
                        ID = 3,
                        Quantity = 10,
                    }
                }
            };

            var postReponse = await _httpClient.PostAsync("api/order/", new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/JSON"));
            var postStringResult = await postReponse.Content.ReadAsStringAsync();

            var isNumber = int.TryParse(postStringResult, out var ID);

            using (new AssertionScope())
            {
                isNumber.Should().BeTrue();
                ID.Should().BeGreaterThan(0);
            }

            var order = await GetOrder(ID);

            order.Should().NotBeNull();

            var deleteReponse = await _httpClient.DeleteAsync($"api/order/{ID}");
            var deleteStringResult = await deleteReponse.Content.ReadAsStringAsync();

            var isBool = bool.TryParse(deleteStringResult, out var value);

            using (new AssertionScope())
            {
                isBool.Should().BeTrue();
                value.Should().BeTrue();
            }

            order = await GetOrder(ID);

            order.Should().BeNull();
        }

        private async Task<AlgoOrder> GetOrder(int id)
        {
            var getReponse = await _httpClient.GetAsync($"api/order/{id}");
            var getStringResult = await getReponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AlgoOrder>(getStringResult);
        }
    }
}
