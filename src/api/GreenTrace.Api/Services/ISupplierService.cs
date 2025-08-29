using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface ISupplierService
{
    Task<IEnumerable<Supplier>> GetByCompanyAsync(Guid companyId);
    Task<Supplier> CreateAsync(Guid companyId, Supplier supplier);
}
