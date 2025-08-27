namespace GreenTrace.Api.ViewModels.Reports;

public record CreateReportViewModel(
    Guid CompanyId,
    int Year,
    string Status,
    string? EsrsVersion,
    bool MaterialityCompleted,
    string? AssuranceLevel);
