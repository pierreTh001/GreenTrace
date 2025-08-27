using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IActivityDataService
{
    Task IngestAsync(ActivityData data);
}
