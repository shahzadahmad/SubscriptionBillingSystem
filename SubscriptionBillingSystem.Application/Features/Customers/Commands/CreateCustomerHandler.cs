using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Application.Features.Customers.Commands
{
    /// <summary>
    /// Handles creation of a new Customer.
    /// 
    /// Flow:
    /// 1. Create Customer aggregate
    /// 2. Add to DbContext (tracking only)
    /// 3. TransactionBehavior will:
    ///    - Save changes
    ///    - Commit transaction
    ///    - Persist any domain events (if added later)
    /// </summary>
    public class CreateCustomerHandler
        : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateCustomerHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(
            CreateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            // Create domain entity
            var customer = new Customer(
                request.Name,
                new Email(request.Email)
            );

            // Track entity in DbContext (no DB call yet)
            await _context.Customers.AddAsync(customer, cancellationToken);

            // ❌ DO NOT call SaveChangesAsync here
            // ✔ TransactionBehavior will handle:
            //    - SaveChanges
            //    - Outbox persistence
            //    - Transaction commit

            return customer.Id;
        }
    }
}