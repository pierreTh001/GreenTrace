using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Services;

public class SubscriptionService(AppDbContext db) : ISubscriptionService
{
    private readonly AppDbContext _db = db;

    public async Task<IEnumerable<SubscriptionPlan>> GetPlansAsync()
        => await _db.SubscriptionPlans.Where(p => p.IsActive).ToListAsync();

    public async Task<Subscription?> GetActiveAsync(Guid userId)
        => await _db.Subscriptions
            .Where(s => s.UserId == userId && s.Status == "Active" && (s.EndsAt == null || s.EndsAt > DateTimeOffset.UtcNow))
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();

    public async Task<bool> HasActiveAsync(Guid userId)
        => await _db.Subscriptions.AnyAsync(s => s.UserId == userId && s.Status == "Active" && (s.EndsAt == null || s.EndsAt > DateTimeOffset.UtcNow));

    public async Task<Subscription> SubscribeAsync(Guid userId, Guid planId)
    {
        var plan = await _db.SubscriptionPlans.FindAsync(planId) ?? throw new KeyNotFoundException("Plan not found");

        // Cancel existing active
        var current = await GetActiveAsync(userId);
        if (current != null)
        {
            current.Status = "Canceled";
            current.EndsAt = DateTimeOffset.UtcNow;
        }

        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = plan.Id,
            Status = "Active",
            StartedAt = DateTimeOffset.UtcNow
        };
        _db.Subscriptions.Add(sub);
        await _db.SaveChangesAsync();
        return sub;
    }
}

