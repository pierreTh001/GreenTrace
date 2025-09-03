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

    public async Task<Subscription?> GetCurrentAsync(Guid userId)
        => await _db.Subscriptions
            .Where(s => s.UserId == userId && (s.EndsAt == null || s.EndsAt > DateTimeOffset.UtcNow))
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();

    public async Task<bool> HasValidThroughAsync(Guid userId)
        => await _db.Subscriptions.AnyAsync(s => s.UserId == userId && (s.EndsAt == null || s.EndsAt > DateTimeOffset.UtcNow));

    public async Task<Subscription> SubscribeAsync(Guid userId, Guid planId)
    {
        var plan = await _db.SubscriptionPlans.FindAsync(planId) ?? throw new KeyNotFoundException("Plan not found");

        // If a current subscription exists (active or canceled but not ended), update it in place:
        var current = await GetCurrentAsync(userId);
        if (current != null)
        {
            current.PlanId = plan.Id;
            current.Status = "Active"; // re-enable auto-renew
            current.EndsAt = null;      // keep same StartedAt (anniversary)
            await _db.SaveChangesAsync();
            return current;
        }

        // Otherwise create a new one starting now
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

    public async Task<Subscription?> CancelAsync(Guid userId)
    {
        var current = await GetCurrentAsync(userId);
        if (current == null) return null;
        // Compute end of current term: next anniversary from StartedAt
        var start = current.StartedAt.UtcDateTime;
        var now = DateTime.UtcNow;
        var end = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, DateTimeKind.Utc);
        while (end <= now) end = end.AddYears(1);
        current.Status = "Canceled";
        current.EndsAt = new DateTimeOffset(end);
        await _db.SaveChangesAsync();
        return current;
    }
}
