using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class Supplier
{
    [Key]
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string? VatNumber { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
