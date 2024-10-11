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
            return await GetCartAsync(userId, cancellationToken)
                ?? await CreateNewCartAsync(userId, cancellationToken);
        }

        private async Task<Cart?> GetCartAsync(string? userId, CancellationToken cancellationToken)
        {
            Cart? cart = null;

            if (!string.IsNullOrEmpty(userId))
            {
                cart = await cartRepository.GetCartByUserIdAsync(userId, cancellationToken);
            }

            if (httpContext != null)
            {
                var cartId = cookieService.GetCartIdFromCookies(httpContext);

                if (!string.IsNullOrEmpty(cartId))
                {
                    cart = await cartRepository.GetCartByCartIdAsync(cartId, cancellationToken);
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
            await cartRepository.StoreCartAsync(cart, cancellationToken);
            return cart;
        }

        public async Task<Cart> StoreCartAsync(IList<ProductSelection> productSelections, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(null, cancellationToken);
            if(cart == null)
            {
                return new Cart();
            }
            
            var productIds = productSelections
                .Select(p => Guid.Parse(p.ProductId))
                .ToList(); 

            var products = await catalogService.GetProductsByIdsAsync(productIds);

            foreach (var selection in productSelections)
            {
                var product = products.First(p => p.Id == selection.ProductId);
                selection.Price = product.Price;
                selection.ProductName = product.Name ?? string.Empty;
            }

            cart.Selections = productSelections.ToList();
            await cartRepository.StoreCartAsync(cart, cancellationToken);
            return cart;
        }

        public async Task DeleteAllProductsFromCartAsync(string? userId, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(userId, cancellationToken);
            await cartRepository.DeleteAllFromCartAsync(cart!.Id, cancellationToken);
        }

        public async Task DeleteProductsFromCartAsync(string? userId, IList<ProductSelection> products, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(userId, cancellationToken);
            await cartRepository.DeleteProductsFromCartAsync(cart!, products, cancellationToken);
        }
    }
}
