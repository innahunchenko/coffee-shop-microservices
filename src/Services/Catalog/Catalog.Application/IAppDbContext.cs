using Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application
{
    public interface IAppDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<Category> Categories { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
