namespace GreenTrace.Api.Infrastructure.Entities;

public class EmissionFactor
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = null!;
    public string? Dataset { get; set; }
    public string? Version { get; set; }
    public string Category { get; set; } = null!;
    public string? Geography { get; set; }
    public string UnitActivity { get; set; } = null!;
    public decimal FactorCo2e { get; set; }
    public string? Breakdown { get; set; }
    public string GwpStandard { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? SourceUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
