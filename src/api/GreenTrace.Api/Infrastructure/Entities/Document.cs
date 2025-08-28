using System.ComponentModel.DataAnnotations;
namespace GreenTrace.Api.Infrastructure.Entities;

public class Document
{
    [Key]
    public Guid Id { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ReportId { get; set; }
    public string FileName { get; set; } = null!;
    public string StorageUrl { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public string? Sha256 { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public byte[] Rv { get; set; } = Array.Empty<byte>();
}
