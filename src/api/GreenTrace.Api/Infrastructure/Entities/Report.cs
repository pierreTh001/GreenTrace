namespace GreenTrace.Api.Infrastructure.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; }
    public string Status { get; set; } = null!;
    public string? EsrsVersion { get; set; }
    public bool MaterialityCompleted { get; set; }
    public string? AssuranceLevel { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public byte[] Rv { get; set; } = Array.Empty<byte>();
}
