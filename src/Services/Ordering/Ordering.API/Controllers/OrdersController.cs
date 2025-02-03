using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Domain.Dtos;
using Ordering.API.Orders.Commands.CreateOrder;
using Security.Services;

namespace Ordering.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender sender;
        private readonly CommandDispatcher commandDispatcher;

        public OrdersController(
            ISender sender, 
            CommandDispatcher commandDispatcher)
        {
            this.sender = sender;
            this.commandDispatcher = commandDispatcher;
        }

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest orderRequest, CancellationToken ct)
        //{
        //    var result = await sender.Send(orderRequest, ct);
        //    return Ok(result);
        //}

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var result = await commandDispatcher.DispatchAsync<List<OrderDto>>();
            return Ok(result);
        }
    }
}
