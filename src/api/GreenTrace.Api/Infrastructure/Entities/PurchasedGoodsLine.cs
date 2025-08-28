using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class PurchasedGoodsLine
{
    [Key]
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? SupplierId { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public string? CategoryLabel { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
