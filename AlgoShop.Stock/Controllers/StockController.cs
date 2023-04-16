using AlgoShop.Stock.Models;
using AlgoShop.Stock.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlgoShop.Stock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var products = await _stockService.GetAsync();

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Product product)
        {
            var isCreated = await _stockService.CreateAsync(product);

            return Ok(isCreated);
        }

        [HttpPut("Increase")]
        public async Task<IActionResult> IncreaseAsync([FromBody] Product product)
        {
            var isIncreased = await _stockService.IncreaseAsync(product);

            return Ok(isIncreased);
        }

        [HttpPut("Decrease")]
        public async Task<IActionResult> DecreaseAsync([FromBody] Product product)
        {
            var isDecreased = await _stockService.DecreaseAsync(product);

            return Ok(isDecreased);
        }

        [HttpPost("Reserve")]
        public async Task<IActionResult> ReserveAsync([FromBody] Reservation reservation)
        {
            if (!reservation.Products.Any())
            {
                return BadRequest("Reservation should have atleast one product");
            }

            var isResearved = await _stockService.ReserveAsync(reservation);

            return Ok(isResearved);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            var isDeleted = await _stockService.RemoveAsync();

            return Ok(isDeleted);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var isDeleted = await _stockService.RemoveByIdAsync(id);

            return Ok(isDeleted);
        }
    }
}
