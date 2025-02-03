namespace Ordering.API.Domain.Models
{
    public enum OrderStatus
    {
        Pending = 1,
        Created = 2,
        Paid = 3,
        PaymentFailed = 4,
        Refunded = 5
    }
}
