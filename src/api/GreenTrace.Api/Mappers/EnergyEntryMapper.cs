using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.Energy;

namespace GreenTrace.Api.Mappers;

public static class EnergyEntryMapper
{
    public static EnergyEntry ToEntity(this CreateEnergyEntryViewModel vm, Guid companyId) => new()
    {
        CompanyId = companyId,
        SiteId = vm.SiteId,
        ReportId = vm.ReportId,
        EnergyType = vm.EnergyType,
        PeriodStart = vm.PeriodStart,
        PeriodEnd = vm.PeriodEnd,
        ConsumptionKwh = vm.ConsumptionKwh,
        RenewableSharePct = vm.RenewableSharePct,
        ContractId = vm.ContractId,
        SourceDocumentId = vm.SourceDocumentId
    };

    public static EnergyEntryViewModel ToViewModel(this EnergyEntry entry) => new(
        entry.Id,
        entry.CompanyId,
        entry.SiteId,
        entry.ReportId,
        entry.EnergyType,
        entry.PeriodStart,
        entry.PeriodEnd,
        entry.ConsumptionKwh,
        entry.RenewableSharePct,
        entry.ContractId,
        entry.SourceDocumentId);
}
