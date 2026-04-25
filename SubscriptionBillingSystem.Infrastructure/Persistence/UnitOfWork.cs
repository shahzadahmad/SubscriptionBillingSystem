using Microsoft.EntityFrameworkCore.Storage;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
                return;

            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);

            await DisposeTransactionAsync();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
                return;

            await _transaction.RollbackAsync(cancellationToken);

            await DisposeTransactionAsync();
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}