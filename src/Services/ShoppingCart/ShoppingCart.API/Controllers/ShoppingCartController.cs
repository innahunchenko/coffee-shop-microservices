using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingCart.API.Dtos;
using ShoppingCart.API.Models;
using ShoppingCart.API.ShoppingCart;

namespace ShoppingCart.API.Controllers
{
    [Route("shopping-cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ISender sender;

        public ShoppingCartController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] string? userId, CancellationToken ct)
        {
            var result = await sender.Send(new GetCartRequest(userId), ct);
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] List<ProductSelection> selections, CancellationToken ct)
        {
            var result = await sender.Send(new StoreCartRequest(selections), ct);
            return Ok(result);
        }


        [HttpPost("checkout")]
        public async Task<IResult> CheckoutCart([FromBody] CartCheckoutDto request, CancellationToken ct)
        {
            var result = await sender.Send(new CheckoutCartRequest(request), ct);
            return result;
        }

        [HttpPost("session/checkout")]
        public async Task<IActionResult> StoreCheckout([FromBody] CartCheckoutDto data)
        {
            HttpContext.Session.SetString("checkout", JsonConvert.SerializeObject(data));
            await HttpContext.Session.CommitAsync();
            return Ok();
        }

        [HttpGet("session/checkout")]
        public async Task<IActionResult> GetCheckout()
        {
            await HttpContext.Session.LoadAsync();
            var data = HttpContext.Session.GetString("checkout");
            return Ok(data);
        }
    }
}
