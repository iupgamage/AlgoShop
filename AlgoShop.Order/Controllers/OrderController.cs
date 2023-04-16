using AlgoShop.Order.Models;
using AlgoShop.Order.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlgoShop.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var orders = await _orderService.GetAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id) 
        {
            var order = await _orderService.GetByIdAsync(id);

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] OrderRequest orderRequest)
        {
            if (orderRequest.Products.Count == 0)
            {
                return BadRequest();
            }

            var orderId = await _orderService.CreateAsync(orderRequest);

            return Ok(orderId);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync() 
        {
            var isDeleted = await _orderService.RemoveAsync();

            return Ok(isDeleted);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var isDeleted = await _orderService.RemoveByIdAsync(id);

            return Ok(isDeleted);
        }
    }
}