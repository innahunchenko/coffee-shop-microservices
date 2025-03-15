using MediatR;
using Messaging.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Domain.Dtos;
using Ordering.API.Mapping;
using Ordering.API.Orders.Commands.CreateOrder;
using Security.Services;
using System.Text.Json;

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

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CartCheckoutEvent request, CancellationToken ct)
        {
            if (request == null)
            {
                return BadRequest("Request body is null or invalid.");
            }

            //logger.LogInformation("Received order request: {@Request}", request);

            var orderRequest = new CreateOrderRequest(request.ToOrderDto());

            var result = await sender.Send(orderRequest, ct);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var result = await commandDispatcher.DispatchAsync<List<OrderDto>>();
            return Ok(result);
        }
    }
}
