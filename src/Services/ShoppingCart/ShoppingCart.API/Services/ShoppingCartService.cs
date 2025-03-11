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
        private readonly ISecureTokenService secureTokenService;
        private readonly string cookieKey = "CoffeeShop.Cart";
        private readonly DateTimeOffset dateTimeOffset = DateTimeOffset.Now.AddDays(7);

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ICatalogService catalogService,
            ICookieService cookieService,             
            ISecureTokenService secureTokenService)
        {
            this.cartRepository = cartRepository;
            this.catalogService = catalogService;
            this.cookieService = cookieService;
            this.secureTokenService = secureTokenService;
        }

        public string GenerateUniqueId() => Guid.NewGuid().ToString();

        public Cart GetOrCreateCart()
        {
            var cart = GetCart()
                ?? CreateNewCart();

            if (cart == null)
                throw new NotFoundException("Cannot find the shopping cart");

            return cart;
        }

        private Cart? GetCart()
        {
            Cart? cart = null;
            string cartId;
            var cartIdFromCookie = cookieService.GetData(cookieKey);
            
            if (cartIdFromCookie == null)
                return cart;

            cartId = secureTokenService.DecodeCartId(cartIdFromCookie!).ToString();
            cart = cartRepository.GetCartByCartId(cartId!);
            return cart;
        }

        private Cart CreateNewCart()
        {
            var cartId = GenerateUniqueId();
            var cart = new Cart { CartId = cartId };
            var encodedCartId = secureTokenService.EncodeCartId(Guid.Parse(cartId));
            cookieService.SetData(cookieKey, encodedCartId, dateTimeOffset);
            cartRepository.StoreCart(cart);
            return cart;
        }

        public Cart StoreCart(IList<ProductSelection> productSelections)
        {
            var cart = GetOrCreateCart();
            
            if (cart == null)
            {
                return new Cart();
            }

            var productIds = productSelections
                .Select(p => Guid.Parse(p.ProductId))
                .ToList();

            var products = catalogService.GetProductsByIdsAsync(productIds).GetAwaiter().GetResult();

            foreach (var selection in productSelections)
            {
                var product = products.First(p => p.Id == selection.ProductId);
                selection.Price = product.Price;
                selection.ProductName = product.Name ?? string.Empty;
            }

            cart.Selections = productSelections.ToList();
            cartRepository.StoreCart(cart);
            return cart;
        }

        public void DeleteCart(Guid shoppingCartId)
        {
            cartRepository.DeleteCart(shoppingCartId);
        }
    }
}
