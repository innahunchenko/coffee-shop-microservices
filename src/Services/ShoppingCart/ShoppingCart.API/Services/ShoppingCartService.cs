using ShoppingCart.API.Exceptions;
using ShoppingCart.API.Models;
using ShoppingCart.API.Repository;

namespace ShoppingCart.API.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository cartRepository;
        private readonly ICatalogService catalogService;
        private readonly HttpContext? httpContext;
        public ShoppingCartService(IShoppingCartRepository cartRepository, 
            IHttpContextAccessor httpContextAccessor,
            ICatalogService catalogService)
        {
            this.cartRepository = cartRepository;
            httpContext = httpContextAccessor.HttpContext;
            this.catalogService = catalogService;
        }

        public async Task<Cart> GetCartAsync(string? userId, CancellationToken cancellationToken)
        {
            Cart? cart = null;

            if (!string.IsNullOrEmpty(userId))
            {
                cart = await cartRepository.GetByUserIdAsync(userId!, cancellationToken);
                return cart; 
            }

            var sessionId = GetSessionId();

            if (string.IsNullOrEmpty(sessionId))
            {
                CreateSessionId();
                return new Cart();
            }

            cart =  await cartRepository.GetBySessionIdAsync(sessionId, cancellationToken);
            return cart;
        }

        public async Task<Cart> StoreCartAsync(Cart cart, CancellationToken cancellationToken)
        {
            Guid.TryParse(cart.Selections.FirstOrDefault().ProductId, out var productId);

            var pro = await catalogService.GetProductByIdAsync(productId);

            cart.SessionId = string.IsNullOrEmpty(cart.UserId) && string.IsNullOrEmpty(cart.SessionId)
                ? GetSessionId()
                : cart.SessionId;

            if (string.IsNullOrEmpty(cart.SessionId))
            {
                cart.SessionId = CreateSessionId();
            }

            var productTasks = cart.Selections
                .Select(async selection =>
                {
                    if (Guid.TryParse(selection.ProductId, out var productId))
                    {
                        var product = await catalogService.GetProductByIdAsync(productId);
                        selection.Price = product.Price;
                        selection.ProductName = product.Name ?? string.Empty;
                    }
                }).ToList();

            await Task.WhenAll(productTasks);

            return await cartRepository.StoreAsync(cart, cancellationToken);
        }

        public Task<bool> DeleteAllProductsAsync(string shoppingCartId, CancellationToken cancellationToken)
        {
            return cartRepository.DeleteAllAsync(shoppingCartId, cancellationToken);
        }

        public async Task<bool> DeleteProductsAsync(string userId, IList<ProductSelection> products, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(userId, cancellationToken);

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

        public string? GetSessionId()
        {
            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is null. This method must be called in an HTTP request context.");
            }

            if (httpContext.Request.Cookies.TryGetValue("SessionId", out var sessionId))
            {
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

            if (httpContext != null)
            {
                httpContext.Response.Cookies.Append("SessionId", sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict
                });
            }

            return sessionId;
        }
    }
}
