using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class Subscription
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PlanId { get; set; }
    public string Status { get; set; } = "Active"; // Active, Canceled, PastDue
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
}

