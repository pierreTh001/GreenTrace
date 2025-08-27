namespace GreenTrace.Api.Infrastructure.Entities;

public class WaterEntry
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? SiteId { get; set; }
    public string Metric { get; set; } = null!;
    public decimal VolumeM3 { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
