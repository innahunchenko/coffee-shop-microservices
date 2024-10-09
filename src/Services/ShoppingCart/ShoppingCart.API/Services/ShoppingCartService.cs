using ShoppingCart.API.Exceptions;
using ShoppingCart.API.Models;
using ShoppingCart.API.Repository;

namespace ShoppingCart.API.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository cartRepository;
        private readonly ICatalogService catalogService;
        private readonly ICookieService cookieService;
        private readonly HttpContext? httpContext;
        public ShoppingCartService(IShoppingCartRepository cartRepository, 
            IHttpContextAccessor httpContextAccessor,
            ICatalogService catalogService,
            ICookieService cookieService)
        {
            this.cartRepository = cartRepository;
            httpContext = httpContextAccessor.HttpContext;
            this.catalogService = catalogService;
            this.cookieService = cookieService;
        }

        public string GenerateUniqueId() => Guid.NewGuid().ToString();

        public async Task<Cart> GetOrCreateCartAsync(string? userId, CancellationToken cancellationToken)
        {
            var cart = await TryGetCartByUserIdOrCartIdAsync(userId, cancellationToken);
            if (cart != null)
            {
                return cart;
            }

            return await CreateNewCartAsync(userId, cancellationToken);
        }

        private async Task<Cart> TryGetCartByUserIdOrCartIdAsync(string? userId, CancellationToken cancellationToken)
        {
            Cart cart = new Cart();

            if (!string.IsNullOrEmpty(userId))
            {
                return await cartRepository.GetCartByUserIdAsync(userId, cancellationToken);
            }

            if (httpContext != null)
            {
                var cartId = cookieService.GetCartIdFromCookies(httpContext);

                if (!string.IsNullOrEmpty(cartId))
                {
                    return await cartRepository.GetCartByCartIdAsync(cartId, cancellationToken);
                }
            }

            return cart;
        }

        private async Task<Cart> CreateNewCartAsync(string? userId, CancellationToken cancellationToken)
        {
            var cartId = GenerateUniqueId();
            var cart = new Cart { UserId = userId, CartId = cartId };
            
            if (httpContext == null)
            {
                throw new InvalidOperationException("HTTP context is not available. Unable to access cookies.");
            }

            cookieService.SetCartIdInCookies(httpContext, cartId);
            return await cartRepository.StoreCartAsync(cart, cancellationToken);
        }

        private async Task<Cart> GetCartAsync(string? userId, CancellationToken cancellationToken)
        {
            var cart = await TryGetCartByUserIdOrCartIdAsync(userId, cancellationToken);

            return cart ?? throw new ShoppingCartNotFoundException(userId ?? "Unknown cart");
        }

        public async Task<Cart> StoreCartAsync(IList<ProductSelection> productSelections, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(null, cancellationToken);

            var productIds = productSelections
                .Where(p => Guid.TryParse(p.ProductId, out _))
                .Select(p => Guid.Parse(p.ProductId))
                .ToList();

            var products = await catalogService.GetProductsByIdsAsync(productIds);

            foreach (var selection in productSelections)
            {
                var product = products.FirstOrDefault(p => p.Id == selection.ProductId);
                if (product != null)
                {
                    selection.Price = product.Price;
                    selection.ProductName = product.Name ?? string.Empty;
                }
            }

            cart.Selections = productSelections.ToList();
            return await cartRepository.StoreCartAsync(cart, cancellationToken);
        }

        public async Task<bool> DeleteAllFromCartAsync(string? userId, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(userId, cancellationToken);
            return await cartRepository.DeleteAllFromCartAsync(cart.Id, cancellationToken);
        }

        public async Task<bool> DeleteProductsAsync(string? userId, IList<ProductSelection> products, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(userId, cancellationToken);
            return await cartRepository.DeleteProductsFromCartAsync(cart, products, cancellationToken);
        }
    }
}
