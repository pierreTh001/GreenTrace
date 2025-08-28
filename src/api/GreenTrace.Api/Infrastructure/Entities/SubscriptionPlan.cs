using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class SubscriptionPlan
{
    [Key]
    public Guid Id { get; set; }
    public string Code { get; set; } = null!; // BASIC, PRO
    public string Name { get; set; } = null!;
    public int PriceCents { get; set; } // 4900, 9900
    public string Currency { get; set; } = "EUR";
    public string Interval { get; set; } = "month"; // monthly for now
    public bool IsActive { get; set; } = true;
}

