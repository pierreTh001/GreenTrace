namespace GreenTrace.Api.Infrastructure.Entities;

public class EnergyEntry
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? ReportId { get; set; }
    public string EnergyType { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal ConsumptionKwh { get; set; }
    public decimal? RenewableSharePct { get; set; }
    public Guid? ContractId { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
