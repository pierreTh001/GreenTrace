using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(Guid id);
    Task<Company> CreateAsync(Company company);
    Task<Company> UpdateAsync(Guid id, Company company);
    Task DeleteAsync(Guid id);
    Task SetParentAsync(Guid companyId, Guid parentCompanyId);
}
