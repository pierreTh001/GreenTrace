using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class KpiValue
{
    [Key]
    public Guid Id { get; set; }
    public Guid KpiId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? ReportId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal Value { get; set; }
    public string Unit { get; set; } = null!;
    public string? DataQuality { get; set; }
    public string? SourceNote { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
