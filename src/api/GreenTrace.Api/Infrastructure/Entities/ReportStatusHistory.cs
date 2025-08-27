namespace GreenTrace.Api.Infrastructure.Entities;

public class ReportStatusHistory
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public string OldStatus { get; set; } = null!;
    public string NewStatus { get; set; } = null!;
    public DateTimeOffset ChangedAt { get; set; }
    public Guid ChangedBy { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
