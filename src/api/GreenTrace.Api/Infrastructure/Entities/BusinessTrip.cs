using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class BusinessTrip
{
    [Key]
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Mode { get; set; } = null!;
    public decimal? DistanceKm { get; set; }
    public int? Nights { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? EmissionFactorId { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
