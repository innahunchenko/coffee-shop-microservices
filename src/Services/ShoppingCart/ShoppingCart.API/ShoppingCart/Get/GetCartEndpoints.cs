using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.API.Models;
using ShoppingCart.API.ShoppingCart.Get;
using ShoppingCart.API.ShoppingCart.Store;

namespace YourNamespace.Controllers
{
    [Route("shopping-cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ISender _sender; 

        public ShoppingCartController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] string? userId, CancellationToken ct)
        {
            var result = await _sender.Send(new GetCartRequest(userId), ct);
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] object selections, CancellationToken ct)
        {
            var result = await _sender.Send(new StoreCartRequest((ProductSelection[])selections), ct);
            return Ok(result);
        }
    }
}
