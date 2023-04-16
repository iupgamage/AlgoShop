using AlgoShop.Orchestrator.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AlgoShop.Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderRequest order)
        {
            var request = JsonConvert.SerializeObject(order);

            var orderServiceClient = httpClientFactory.CreateClient("Order");
            var orderServiceResponse = await orderServiceClient.PostAsync("api/order", new StringContent(request, Encoding.UTF8, "application/JSON"));
            var orderId = await orderServiceResponse.Content.ReadAsStringAsync();

            try
            {
                var stockServiceClient = httpClientFactory.CreateClient("Stock");
                var stockServiceResponse = await stockServiceClient.PostAsync("api/stock/reserve", new StringContent(request, Encoding.UTF8, "application/JSON"));
                var content = await stockServiceResponse.Content.ReadAsStringAsync();

                if (!stockServiceResponse.IsSuccessStatusCode)
                {
                    await orderServiceClient.DeleteAsync($"api/order/{orderId}");

                    return Ok(new OrderResponse { Success = false, Reason = content });
                }

            }
            catch (Exception ex)
            {
                await orderServiceClient.DeleteAsync($"api/order/{orderId}");

                return Ok(new OrderResponse { Success = false, Reason = ex.Message ?? ex.InnerException?.Message });
            }

            return Ok(new OrderResponse { Success = true, Reason = "Order successfully handled" });
        }
    }
}
