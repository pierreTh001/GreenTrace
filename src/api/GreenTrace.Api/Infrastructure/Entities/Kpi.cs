using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class Kpi
{
    [Key]
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public string? TopicCode { get; set; }
    public string? Metadata { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
