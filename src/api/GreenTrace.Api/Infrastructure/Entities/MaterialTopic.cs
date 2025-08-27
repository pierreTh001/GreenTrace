namespace GreenTrace.Api.Infrastructure.Entities;

public class MaterialTopic
{
    public Guid Id { get; set; }
    public Guid AssessmentId { get; set; }
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
    public decimal? ImpactScore { get; set; }
    public decimal? FinancialScore { get; set; }
    public bool IsMaterial { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
