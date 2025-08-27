using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public class ActivityDataService : IActivityDataService
{
    private readonly AppDbContext _db;

    public ActivityDataService(AppDbContext db)
    {
        _db = db;
    }

    public async Task IngestAsync(ActivityData data)
    {
        if (data.Id == Guid.Empty) data.Id = Guid.NewGuid();
        data.CreatedAt = DateTimeOffset.UtcNow;
        data.UpdatedAt = data.CreatedAt;
        _db.ActivityData.Add(data);
        await _db.SaveChangesAsync();
    }
}
