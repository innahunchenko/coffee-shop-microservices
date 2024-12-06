using Security.Models;
using Security.Services;

namespace Auth.API.Services
{
    public class MenuService
    {
        private readonly IUserContext userContext;

        private static readonly List<MenuItem> _menuTable = new List<MenuItem>
        {
            new MenuItem { Id = "catalog", Name = "Catalog", Roles = [Roles.ADMIN] },
            new MenuItem { Id = "orders", Name = "Orders", Roles = [Roles.USER, Roles.ADMIN] },
            new MenuItem { Id = "returnToShop", Name = "Continue shopping", Roles = [Roles.USER] },
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
