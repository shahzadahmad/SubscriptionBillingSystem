using MediatR;

namespace SubscriptionBillingSystem.Application.Common.Interfaces
{
    /// <summary>
    /// Non-generic marker interface (used ONLY for pipeline filtering)
    /// </summary>
    public interface ICommand
    {
    }

    /// <summary>
    /// Generic command interface for MediatR
    /// </summary>
    public interface ICommand<TResponse> : IRequest<TResponse>, ICommand
    {
    }
}