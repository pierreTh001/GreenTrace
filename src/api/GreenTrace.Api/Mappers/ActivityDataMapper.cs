using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.ActivityData;

namespace GreenTrace.Api.Mappers;

public static class ActivityDataMapper
{
    public static ActivityData ToEntity(this ActivityDataViewModel vm) => new()
    {
        CompanyId = vm.CompanyId,
        SiteId = vm.SiteId,
        ReportId = vm.ReportId,
        ActivityType = vm.ActivityType,
        ScopeHint = vm.ScopeHint,
        Scope3Category = vm.Scope3Category,
        PeriodStart = vm.PeriodStart,
        PeriodEnd = vm.PeriodEnd,
        Quantity = vm.Quantity,
        UnitActivity = vm.UnitActivity,
        EmissionFactorId = vm.EmissionFactorId,
        DataQuality = vm.DataQuality,
        SourceDocumentId = vm.SourceDocumentId,
        Notes = vm.Notes,
        Extra = vm.Extra
    };
}
