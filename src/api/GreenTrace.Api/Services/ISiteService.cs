using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface ISiteService
{
    Task<IEnumerable<CompanySite>> GetByCompanyAsync(Guid companyId);
    Task<CompanySite> CreateAsync(Guid companyId, CompanySite site);
}
