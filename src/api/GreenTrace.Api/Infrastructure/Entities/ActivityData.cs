namespace GreenTrace.Api.Infrastructure.Entities;

public class ActivityData
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? ReportId { get; set; }
    public string ActivityType { get; set; } = null!;
    public string? ScopeHint { get; set; }
    public int? Scope3Category { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal Quantity { get; set; }
    public string UnitActivity { get; set; } = null!;
    public Guid? EmissionFactorId { get; set; }
    public string? DataQuality { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? Notes { get; set; }
    public string? Extra { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
