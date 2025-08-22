namespace GreenTrace.Api.Domain;

public class Company
{
    public int Id { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
