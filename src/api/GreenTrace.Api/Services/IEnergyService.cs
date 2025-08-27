using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IEnergyService
{
    Task<IEnumerable<EnergyEntry>> GetEntriesAsync(Guid companyId);
    Task<EnergyEntry> AddEntryAsync(Guid companyId, EnergyEntry entry);
}
