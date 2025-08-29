using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class Company
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LegalForm { get; set; }
    public string? Siren { get; set; }
    public string? Siret { get; set; }
    public string? VatNumber { get; set; }
    public string? NaceCode { get; set; }
    public string? Website { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? HqCountry { get; set; }
    public int? EmployeesCount { get; set; }
    public string? ConsolidationMethod { get; set; }
    public string? EsgContactEmail { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public byte[] Rv { get; set; } = Array.Empty<byte>();
}
