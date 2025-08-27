namespace GreenTrace.Api.ViewModels.Energy;

public record EnergyEntryViewModel(
    Guid Id,
    Guid CompanyId,
    Guid? SiteId,
    Guid? ReportId,
    string EnergyType,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    decimal ConsumptionKwh,
    decimal? RenewableSharePct,
    Guid? ContractId,
    Guid? SourceDocumentId);
