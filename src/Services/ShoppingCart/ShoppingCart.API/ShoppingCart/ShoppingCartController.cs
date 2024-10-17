using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.API.Models;

namespace ShoppingCart.API.ShoppingCart
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
        public async Task<IActionResult> AddToCart([FromBody] List<ProductSelection> selections, CancellationToken ct)
        {
            var result = await _sender.Send(new StoreCartRequest(selections), ct);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutCart([FromBody] CheckoutCartRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(request, ct);
            return Ok(result);
        }
    }
}
