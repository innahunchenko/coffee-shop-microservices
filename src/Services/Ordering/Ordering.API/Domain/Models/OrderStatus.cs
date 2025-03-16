namespace Ordering.API.Domain.Models
{
    public enum OrderStatus
    {
        Created = 1,
        Paid = 2,
        PaymentFailed = 3,
        Refunded = 4
    }
}
