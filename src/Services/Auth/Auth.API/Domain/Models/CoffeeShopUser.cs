using Microsoft.AspNetCore.Identity;

namespace Auth.API.Domain.Models
{
    public class CoffeeShopUser : IdentityUser
    {
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; }
    }
}
