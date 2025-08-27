namespace GreenTrace.Api.Infrastructure.Entities;

public class MaterialityAssessment
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public string Methodology { get; set; } = null!;
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
