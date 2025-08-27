using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Services;

public class DocumentService : IDocumentService
{
    private readonly AppDbContext _db;

    public DocumentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Document> UploadAsync(Document document)
    {
        if (document.Id == Guid.Empty) document.Id = Guid.NewGuid();
        document.CreatedAt = DateTimeOffset.UtcNow;
        document.UpdatedAt = document.CreatedAt;
        _db.Documents.Add(document);
        await _db.SaveChangesAsync();
        return document;
    }

    public Task<Document?> GetAsync(Guid id)
        => _db.Documents.FirstOrDefaultAsync(d => d.Id == id);
}
