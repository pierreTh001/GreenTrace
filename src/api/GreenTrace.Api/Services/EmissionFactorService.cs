using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GreenTrace.Api.Services;

public class EmissionFactorService : IEmissionFactorService
{
    private readonly AppDbContext _db;

    public EmissionFactorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EmissionFactor>> ListAsync()
        => await _db.EmissionFactors.ToListAsync();

    public async Task<IEnumerable<EmissionFactor>> SearchAsync(string query)
        => await _db.EmissionFactors
            .Where(f => f.Provider.Contains(query) ||
                        f.Category.Contains(query) ||
                        (f.Geography ?? "").Contains(query))
            .ToListAsync();
}
