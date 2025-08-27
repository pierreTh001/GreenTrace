using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IEmissionFactorService
{
    Task<IEnumerable<EmissionFactor>> ListAsync();
    Task<IEnumerable<EmissionFactor>> SearchAsync(string query);
}
