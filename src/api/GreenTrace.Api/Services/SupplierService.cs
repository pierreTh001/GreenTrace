using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _db;

    public SupplierService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Supplier>> GetByCompanyAsync(Guid companyId)
        => await _db.Suppliers.Where(s => s.CompanyId == companyId).ToListAsync();

    public async Task<Supplier> CreateAsync(Guid companyId, Supplier supplier)
    {
        if (supplier.Id == Guid.Empty) supplier.Id = Guid.NewGuid();
        supplier.CompanyId = companyId;
        supplier.CreatedAt = DateTimeOffset.UtcNow;
        supplier.UpdatedAt = supplier.CreatedAt;
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return supplier;
    }
}
