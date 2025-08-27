using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IEmissionService
{
    Task<IEnumerable<EmissionResult>> CalculateAsync(Guid reportId);
}
