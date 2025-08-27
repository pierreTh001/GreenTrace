namespace GreenTrace.Api.Infrastructure.Entities;

public class EmissionResult
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? ReportId { get; set; }
    public Guid? ActivityId { get; set; }
    public string Scope { get; set; } = null!;
    public int? Scope3Category { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal ValueTco2e { get; set; }
    public string Method { get; set; } = null!;
    public string GwpStandard { get; set; } = null!;
    public string? Breakdown { get; set; }
    public DateTimeOffset CalculatedAt { get; set; }
    public Guid CalculatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
