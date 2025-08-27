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

    public Task<IEnumerable<Supplier>> GetAllAsync()
        => _db.Suppliers.ToListAsync();

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        if (supplier.Id == Guid.Empty) supplier.Id = Guid.NewGuid();
        supplier.CreatedAt = DateTimeOffset.UtcNow;
        supplier.UpdatedAt = supplier.CreatedAt;
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return supplier;
    }
}
