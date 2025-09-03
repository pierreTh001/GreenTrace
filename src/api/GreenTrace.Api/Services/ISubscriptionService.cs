using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface ISubscriptionService
{
    Task<IEnumerable<SubscriptionPlan>> GetPlansAsync();
    Task<Subscription?> GetActiveAsync(Guid userId);
    Task<bool> HasActiveAsync(Guid userId);
    Task<Subscription?> GetCurrentAsync(Guid userId);
    Task<bool> HasValidThroughAsync(Guid userId);
    Task<Subscription> SubscribeAsync(Guid userId, Guid planId);
    Task<Subscription?> CancelAsync(Guid userId);
}
