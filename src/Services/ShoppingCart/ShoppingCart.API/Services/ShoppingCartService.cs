using ShoppingCart.API.Exceptions;
using ShoppingCart.API.Models;
using ShoppingCart.API.Repository;

namespace ShoppingCart.API.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository cartRepository;
        private readonly HttpContext? httpContext;
        private string? sessionId;
        public ShoppingCartService(IShoppingCartRepository cartRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.cartRepository = cartRepository;
            httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<Cart> GetAsync(string? userId, CancellationToken cancellationToken)
        {
            Cart? cart = null;

            if (string.IsNullOrEmpty(userId))
            {
                cart = await cartRepository.GetByUserIdAsync(userId!, cancellationToken);
            }

            if (cart != null)
                return cart;

            var sessionId = GetSessionId();

            return await cartRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        }

        public Task<Cart> StoreAsync(Cart cart, CancellationToken cancellationToken)
        {
            cart.SessionId = string.IsNullOrEmpty(cart.UserId) && string.IsNullOrEmpty(cart.SessionId) ? CreateSessionId() : cart.SessionId;
            cart.Id = Guid.NewGuid();
            return cartRepository.StoreAsync(cart, cancellationToken);           
        }

        public Task<bool> DeleteAllProductsAsync(string shoppingCartId, CancellationToken cancellationToken)
        {
            return cartRepository.DeleteAllAsync(shoppingCartId, cancellationToken);
        }

        public async Task<bool> DeleteProductsAsync(string userId, IList<ProductSelection> products, CancellationToken cancellationToken)
        {
            var cart = await GetAsync(userId, cancellationToken);

            if (cart == null)
            {
                throw new ShoppingCartNotFoundException(userId);
            }

            return await cartRepository.DeleteProductsAsync(cart, products, cancellationToken);
        }

        public async Task MergeGuestCartWithUserCartAsync(string guestSessionId, string userId, CancellationToken cancellationToken)
        {
            var guestCart = await cartRepository.GetBySessionIdAsync(guestSessionId, cancellationToken);
            var userCart = await cartRepository.GetByUserIdAsync(userId, cancellationToken);

            if (guestCart != null)
            {
                if (userCart != null)
                {
                    foreach (var guestCartItem in guestCart.Selections)
                    {
                        var existingItem = userCart.Selections.FirstOrDefault(p => p.ProductId == guestCartItem.ProductId);
                        if (existingItem != null)
                        {
                            existingItem.Quantity += guestCartItem.Quantity;
                        }
                        else
                        {
                            userCart.Selections.Add(guestCartItem);
                        }
                    }

                    await cartRepository.StoreAsync(userCart, cancellationToken);

                    await cartRepository.DeleteAllAsync(guestCart.Id.ToString(), cancellationToken);
                }
                else
                {
                    guestCart.UserId = userId;
                    guestCart.SessionId = null;
                    await cartRepository.StoreAsync(guestCart, cancellationToken);
                }
            }
        }

        public string GetSessionId()
        {
            if (this.sessionId != null)
            {
                return this.sessionId;
            }

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is null. This method must be called in an HTTP request context.");
            }

            if (httpContext.Request.Cookies.TryGetValue("SessionId", out var sessionId))
            {
                this.sessionId = sessionId;
                return sessionId;
            }

            return string.Empty;
        }

        public string CreateSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is null. This method must be called in an HTTP request context.");
            }

            httpContext.Response.Cookies.Append("SessionId", sessionId, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(30)
            });

            return sessionId;
        }

    }
}
