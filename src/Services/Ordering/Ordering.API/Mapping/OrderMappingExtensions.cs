using Foundation.Abstractions.Models;
using Messaging.Events;
using Ordering.API.Domain.Dtos;
using Ordering.API.Domain.Models;
using Ordering.API.Domain.ValueObjects.OrderItemObjects;
using Ordering.API.Domain.ValueObjects.OrderObjects;

namespace Ordering.API.Mapping
{
    public static class OrderMappingExtensions
    {
        public static Order ToOrder(this OrderDto orderDto) 
        {
            var shippingAddress = Address.From(
                orderDto.ShippingAddress.AddressLine,
                orderDto.ShippingAddress.Country,
                orderDto.ShippingAddress.State,
                orderDto.ShippingAddress.ZipCode);

            var payment = Payment.From(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.CVV);
            var order = Order.Create(shippingAddress, payment, Enum.Parse<OrderStatus>(orderDto.OrderStatus), orderDto.PhoneNumber, orderDto.Email, orderDto.UserId);

            foreach (var orderItemDto in orderDto.OrderItems)
            {
                order.AddItem(ProductId.From(Guid.Parse(orderItemDto.ProductId)),
                                ProductName.From(orderItemDto.ProductName),
                                orderItemDto.Quantity,
                                orderItemDto.Price);
            }

            return order;
        }

        public static OrderDto ToOrderDto(this CartCheckoutEvent src)
        {
            var orderDto = new OrderDto();
            orderDto.PhoneNumber = PhoneNumber.From(src.PhoneNumber);
            orderDto.Email = Email.From(src.EmailAddress);
            orderDto.UserId = src.UserId;

            foreach (var selection in src.ProductSelections)
            {
                var orderItemDto = new OrderItemDto()
                {
                    ProductId = selection.ProductId,
                    ProductName = selection.ProductName,
                    Quantity = selection.Quantity,
                    Price = selection.Price
                };

                orderDto.OrderItems.Add(orderItemDto);
            }

            orderDto.ShippingAddress = new AddressDto()
            {
                AddressLine = src.AddressLine,
                Country = src.Country,
                State = src.State,
                ZipCode = src.ZipCode
            };

            orderDto.Payment = new PaymentDto()
            {
                CardName = src.CardName,
                CardNumber = src.CardNumber,
                Expiration = src.Expiration,
                CVV = src.CVV
            };

            return orderDto;
        }

        public static OrderDto ToOrderDto(this Order order)
        {
            var orderDto = new OrderDto();
            orderDto.OrderStatus = order.Status.ToString();
            orderDto.OrderName = order.OrderName.Value;
            orderDto.TotalPrice = order.TotalPrice;
            orderDto.PhoneNumber = order.PhoneNumber;
            orderDto.Email = order.Email;
            orderDto.UserId = order.UserId;
            orderDto.CreatedAt = order.CreatedAt;

            foreach (var item in order.OrderItems)
            {
                var orderItemDto = new OrderItemDto()
                {
                    OrderId = item.OrderId.Value.ToString(),
                    Price = item.Price,
                    ProductId = item.ProductId.Value.ToString(),
                    ProductName = item.ProductName.Value.ToString(),
                    Quantity = item.Quantity
                };

                orderDto.OrderItems.Add(orderItemDto);
            }

            orderDto.ShippingAddress = new AddressDto()
            {
                AddressLine = order.ShippingAddress.AddressLine,
                Country = order.ShippingAddress.Country,
                State = order.ShippingAddress.State,
                ZipCode = order.ShippingAddress.ZipCode
            };

            orderDto.Payment = new PaymentDto()
            {
                CardName = order.Payment.CardName,
                CardNumber = order.Payment.CardNumber,
                CVV = order.Payment.CVV,
                Expiration = order.Payment.Expiration
            };

            return orderDto;
        }
    }
}
