using Foundation.Abstractions.Services;
using Foundation.Exceptions;
using ShoppingCart.API.Models;
using ShoppingCart.API.Repository;

namespace ShoppingCart.API.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository cartRepository;
        private readonly ICatalogService catalogService;
        private readonly ICookieService cookieService;
        private readonly string cookieKey = "CoffeeShop.Cart";
        private readonly DateTimeOffset dateTimeOffset = DateTimeOffset.Now.AddDays(7);

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ICatalogService catalogService,
            ICookieService cookieService)
        {
            this.cartRepository = cartRepository;
            this.catalogService = catalogService;
            this.cookieService = cookieService;
        }

        public string GenerateUniqueId() => Guid.NewGuid().ToString();

        public async Task<Cart> GetOrCreateCartAsync(CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(cancellationToken)
                ?? await CreateNewCartAsync(cancellationToken);

            if (cart == null)
                throw new NotFoundException("Cannot find the shopping cart");

            return cart;
        }

        private async Task<Cart?> GetCartAsync(CancellationToken cancellationToken)
        {
            Cart? cart = null;

            var cartId = cookieService.GetData(cookieKey);

            if (!string.IsNullOrEmpty(cartId))
            {
                cart = await cartRepository.GetCartByCartIdAsync(cartId, cancellationToken);
            }

            return cart;
        }

        private async Task<Cart> CreateNewCartAsync(CancellationToken cancellationToken)
        {
            var cartId = GenerateUniqueId();
            var cart = new Cart { CartId = cartId };
            cookieService.SetData(cookieKey, cartId, dateTimeOffset);
            await cartRepository.StoreCartAsync(cart, cancellationToken);
            return cart;
        }

        public async Task<Cart> StoreCartAsync(IList<ProductSelection> productSelections, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(cancellationToken);
            if (cart == null)
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

        public async Task DeleteCartAsync(Guid shoppingCartId, CancellationToken cancellationToken)
        {
            await cartRepository.DeleteCartAsync(shoppingCartId, cancellationToken);
        }
    }
}
