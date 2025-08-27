namespace GreenTrace.Api.Infrastructure.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public DateTimeOffset PerformedAt { get; set; }
    public Guid PerformedBy { get; set; }
    public string Action { get; set; } = null!;
    public string Entity { get; set; } = null!;
    public Guid? EntityId { get; set; }
    public string? Diff { get; set; }
    public string? IpAddress { get; set; }
}
