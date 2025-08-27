namespace GreenTrace.Api.ViewModels.Documents;

public record UploadDocumentViewModel(
    Guid? CompanyId,
    Guid? ReportId,
    string FileName,
    string StorageUrl,
    string MimeType,
    string? Sha256);
