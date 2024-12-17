namespace ShoppingCart.API.Services
{
    public interface ISecureTokenService
    {
        string EncodeCartId(Guid cartId);
        Guid DecodeCartId(string encodedCartId);
    }
}
