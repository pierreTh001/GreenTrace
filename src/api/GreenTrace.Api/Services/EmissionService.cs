using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GreenTrace.Api.Services;

public class EmissionService : IEmissionService
{
    private readonly AppDbContext _db;

    public EmissionService(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<EmissionResult>> CalculateAsync(Guid reportId)
        => _db.EmissionResults.Where(r => r.ReportId == reportId).ToListAsync();
}
