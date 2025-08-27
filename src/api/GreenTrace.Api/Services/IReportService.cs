using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IReportService
{
    Task<IEnumerable<Report>> GetAllAsync();
    Task<Report?> GetByIdAsync(Guid id);
    Task<Report> CreateAsync(Report report);
    Task LockAsync(Guid id);
    Task UnlockAsync(Guid id);
    Task SignOffAsync(Guid id);
}
