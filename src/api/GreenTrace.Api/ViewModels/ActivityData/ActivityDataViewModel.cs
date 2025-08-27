namespace GreenTrace.Api.ViewModels.ActivityData;

public record ActivityDataViewModel(
    Guid CompanyId,
    Guid? SiteId,
    Guid? ReportId,
    string ActivityType,
    string? ScopeHint,
    int? Scope3Category,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    decimal Quantity,
    string UnitActivity,
    Guid? EmissionFactorId,
    string? DataQuality,
    Guid? SourceDocumentId,
    string? Notes,
    string? Extra);
