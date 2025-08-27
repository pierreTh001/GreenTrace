using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface ISupplierService
{
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier> CreateAsync(Supplier supplier);
}
