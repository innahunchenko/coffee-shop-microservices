using Auth.API.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Auth.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext appDbContext;
        private IDbContextTransaction? currentTransaction;

        public UnitOfWork(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            currentTransaction ??= await appDbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (currentTransaction != null)
            {
                await currentTransaction.CommitAsync(cancellationToken);
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (currentTransaction != null)
            {
                await currentTransaction.RollbackAsync(cancellationToken);
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
