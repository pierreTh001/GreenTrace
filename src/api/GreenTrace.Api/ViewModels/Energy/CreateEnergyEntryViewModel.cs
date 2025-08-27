namespace GreenTrace.Api.ViewModels.Energy;

public record CreateEnergyEntryViewModel(
    Guid? SiteId,
    Guid? ReportId,
    string EnergyType,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    decimal ConsumptionKwh,
    decimal? RenewableSharePct,
    Guid? ContractId,
    Guid? SourceDocumentId);
