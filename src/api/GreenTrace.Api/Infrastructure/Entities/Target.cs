namespace GreenTrace.Api.Infrastructure.Entities;

public class Target
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string? TopicCode { get; set; }
    public string? MetricCode { get; set; }
    public int? BaselineYear { get; set; }
    public decimal? BaselineValue { get; set; }
    public int TargetYear { get; set; }
    public decimal? TargetValue { get; set; }
    public string? Unit { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
