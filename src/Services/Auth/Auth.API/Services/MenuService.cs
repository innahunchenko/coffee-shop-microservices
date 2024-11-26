using Auth.API.Domain.Models;

namespace Auth.API.Services
{
    public class MenuService
    {
        private readonly IUserContext userContext;

        private static readonly List<MenuItem> _menuTable = new List<MenuItem>
        {
            new MenuItem { Id = "manageCatalog", Name = "Manage Catalog", Roles = [Roles.ADMIN] },
            new MenuItem { Id = "manageUsers", Name = "Manage Users", Roles = [Roles.ADMIN] },
            new MenuItem { Id = "manageUserOrders", Name = "Manage User Orders", Roles = [Roles.ADMIN] },
            new MenuItem { Id = "orderHistory", Name = "Order History", Roles = [Roles.USER] },
            new MenuItem { Id = "profile", Name = "Profile", Roles = [Roles.ADMIN, Roles.USER] },
            new MenuItem { Id = "returnToShop", Name = "Return to Shop", Roles = [Roles.USER] },
            new MenuItem { Id = "signOut", Name = "Sign out", Roles = [Roles.ADMIN, Roles.USER] }
        };

        public MenuService(IUserContext userContext)
        {
            this.userContext = userContext;
        }

        public List<MenuItem> GetUserMenu()
        {
            var userRole = userContext.GetUserRole();

            return _menuTable
                .Where(item => item.Roles.Contains(userRole))
                .ToList();
        }
    }

    public class MenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Roles[] Roles { get; set; }
    }
}
