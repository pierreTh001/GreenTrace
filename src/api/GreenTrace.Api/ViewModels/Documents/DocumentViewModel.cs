namespace GreenTrace.Api.ViewModels.Documents;

public record DocumentViewModel(
    Guid Id,
    Guid? CompanyId,
    Guid? ReportId,
    string FileName,
    string StorageUrl,
    string MimeType,
    string? Sha256);
