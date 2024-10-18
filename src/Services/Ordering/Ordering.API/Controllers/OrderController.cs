using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Orders.Commands.CreateOrder;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;

        public OrderController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest orderRequest, CancellationToken ct)
        {
            var result = await _sender.Send(orderRequest, ct);
            return Ok(result);
        }
    }
}
