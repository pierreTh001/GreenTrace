using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GreenTrace.Api.Services;

public class EnergyService : IEnergyService
{
    private readonly AppDbContext _db;

    public EnergyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EnergyEntry>> GetEntriesAsync(Guid companyId)
        => await _db.EnergyEntries.Where(e => e.CompanyId == companyId).ToListAsync();

    public async Task<EnergyEntry> AddEntryAsync(Guid companyId, EnergyEntry entry)
    {
        if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();
        entry.CompanyId = companyId;
        entry.CreatedAt = DateTimeOffset.UtcNow;
        entry.UpdatedAt = entry.CreatedAt;
        _db.EnergyEntries.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }
}
