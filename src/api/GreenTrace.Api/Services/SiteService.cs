using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GreenTrace.Api.Services;

public class SiteService : ISiteService
{
    private readonly AppDbContext _db;

    public SiteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CompanySite>> GetByCompanyAsync(Guid companyId)
        => await _db.CompanySites.Where(s => s.CompanyId == companyId).ToListAsync();

    public async Task<CompanySite> CreateAsync(Guid companyId, CompanySite site)
    {
        if (site.Id == Guid.Empty) site.Id = Guid.NewGuid();
        site.CompanyId = companyId;
        site.CreatedAt = DateTimeOffset.UtcNow;
        site.UpdatedAt = site.CreatedAt;
        _db.CompanySites.Add(site);
        await _db.SaveChangesAsync();
        return site;
    }
}
