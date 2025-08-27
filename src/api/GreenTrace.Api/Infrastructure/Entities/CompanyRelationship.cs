namespace GreenTrace.Api.Infrastructure.Entities;

public class CompanyRelationship
{
    public Guid ParentCompanyId { get; set; }
    public Guid ChildCompanyId { get; set; }
    public decimal? EquitySharePct { get; set; }
    public DateTime? Since { get; set; }
    public DateTime? Until { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
