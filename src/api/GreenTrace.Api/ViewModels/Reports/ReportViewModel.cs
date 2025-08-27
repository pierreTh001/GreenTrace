namespace GreenTrace.Api.ViewModels.Reports;

public record ReportViewModel(
    Guid Id,
    Guid CompanyId,
    int Year,
    string Status,
    string? EsrsVersion,
    bool MaterialityCompleted,
    string? AssuranceLevel);
