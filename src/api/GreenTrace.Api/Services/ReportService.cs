using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<Report>> GetAllAsync()
        => _db.Reports.ToListAsync();

    public Task<Report?> GetByIdAsync(Guid id)
        => _db.Reports.FindAsync(id).AsTask();

    public async Task<Report> CreateAsync(Report report)
    {
        if (report.Id == Guid.Empty) report.Id = Guid.NewGuid();
        report.CreatedAt = DateTimeOffset.UtcNow;
        report.UpdatedAt = report.CreatedAt;
        _db.Reports.Add(report);
        await _db.SaveChangesAsync();
        return report;
    }

    public async Task LockAsync(Guid id)
    {
        var report = await _db.Reports.FindAsync(id) ?? throw new KeyNotFoundException();
        await ChangeStatusAsync(report, "Locked");
    }

    public async Task UnlockAsync(Guid id)
    {
        var report = await _db.Reports.FindAsync(id) ?? throw new KeyNotFoundException();
        await ChangeStatusAsync(report, "Draft");
    }

    public async Task SignOffAsync(Guid id)
    {
        var report = await _db.Reports.FindAsync(id) ?? throw new KeyNotFoundException();
        await ChangeStatusAsync(report, "SignedOff");
    }

    private async Task ChangeStatusAsync(Report report, string newStatus)
    {
        var oldStatus = report.Status;
        report.Status = newStatus;
        report.UpdatedAt = DateTimeOffset.UtcNow;
        _db.ReportStatusHistories.Add(new ReportStatusHistory
        {
            Id = Guid.NewGuid(),
            ReportId = report.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = DateTimeOffset.UtcNow,
            ChangedBy = Guid.Empty,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        });
        await _db.SaveChangesAsync();
    }
}
