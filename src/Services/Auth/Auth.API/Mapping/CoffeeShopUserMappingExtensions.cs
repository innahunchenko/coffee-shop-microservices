using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;

namespace Auth.API.Mapping
{
    public static class CoffeeShopUserMappingExtensions
    {
        public static CoffeeShopUser ToUser(this CoffeeShopUserDto src) 
        {
            DateTime.TryParse(src.DateOfBirth, out var dateOfBirth);

            return new()
            {
                FirstName = src.FirstName!,
                LastName = src.LastName!,
                DateOfBirth = dateOfBirth,
                UserName = src.UserName,
                Email = src.Email,
                NormalizedEmail = src.Email!.ToLower(),
                PhoneNumber = src.PhoneNumber,
                NormalizedUserName = src.UserName!.ToLower()
            };
        }
    }
}
