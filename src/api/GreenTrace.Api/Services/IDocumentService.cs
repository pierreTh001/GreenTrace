using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IDocumentService
{
    Task<Document> UploadAsync(Document document);
    Task<Document?> GetAsync(Guid id);
}
