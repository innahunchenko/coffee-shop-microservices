using LanguageExt.Pipes;
using MediatR;
using ShoppingCart.API.Exceptions;
using ShoppingCart.API.Models;
using ShoppingCart.API.Repository;
using static System.Net.WebRequestMethods;

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

        public string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<Cart> GetOrCreateCartAsync(string? userId, CancellationToken cancellationToken)
        {
            Cart cart = new Cart();

            if (!string.IsNullOrEmpty(userId))
            {
                cart = await cartRepository.GetCartByUserIdAsync(userId, cancellationToken);
                
                if (cart == null)
                {
                    cart = new Cart() { UserId =  userId };
                    cart = await cartRepository.StoreCartAsync(cart, cancellationToken);
                }

                return cart;
            }

            if (httpContext != null && httpContext.Request.Cookies.TryGetValue("cartId", out string? cartId)) 
            {
                if (!string.IsNullOrEmpty(cartId))
                { 
                    return await cartRepository.GetCartByCartIdAsync(cartId, cancellationToken); 
                }
            }

            if (httpContext != null)
            {
                cartId = Guid.NewGuid().ToString();
                cart = new Cart() { CartId = cartId };
                AddCartIdToCookie(httpContext, cartId);
                cart = await cartRepository.StoreCartAsync(cart, cancellationToken);
            }

            return cart;
        }

        public async Task<Cart> StoreCartAsync(IList<ProductSelection> productSelections, CancellationToken cancellationToken)
        {
            var cart = await GetCartAsync(null, cancellationToken);

            var productTasks = productSelections
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

        private async Task<Cart> GetCartAsync(string? userId, CancellationToken cancellationToken)
        {
            Cart? cart = null;
            string? cartId = null;

            if (!string.IsNullOrEmpty(userId))
            {
                cart = await cartRepository.GetCartByUserIdAsync(userId, cancellationToken);
            }

            if (httpContext != null && httpContext.Request.Cookies.TryGetValue("cartId", out cartId))
            {
                if (!string.IsNullOrEmpty(cartId))
                {
                    cart = await cartRepository.GetCartByCartIdAsync(cartId, cancellationToken);
                }
            }

            if (cart == null)
            {
                throw new ShoppingCartNotFoundException(userId ?? cartId ?? "");
            }

            return cart;
        }

        public void AddCartIdToCookie(HttpContext httpContext, string cartId)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMonths(6),
            //HttpOnly = true,
            //Secure = false,
            SameSite = SameSiteMode.Lax
            };

            httpContext.Response.Cookies.Append("cartId", cartId, cookieOptions);
        }

        /*
        public async Task MergeGuestCartWithUserCartAsync(string guestSessionId, string userId, CancellationToken cancellationToken)
        {
            var guestCart = await cartRepository.GetCartByCartIdAsync(guestSessionId, cancellationToken);
            var userCart = await cartRepository.GetCartByUserIdAsync(userId, cancellationToken);

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

                    await cartRepository.StoreCartAsync(userCart, cancellationToken);

                    await cartRepository.DeleteAllFromCartAsync(guestCart.Id.ToString(), cancellationToken);
                }
                else
                {
                    guestCart.UserId = userId;
                    guestCart.SessionId = null;
                    await cartRepository.StoreCartAsync(guestCart, cancellationToken);
                }
            }
        }
        */
    }
}
