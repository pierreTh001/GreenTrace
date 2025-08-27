using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.Reports;

namespace GreenTrace.Api.Mappers;

public static class ReportMapper
{
    public static Report ToEntity(this CreateReportViewModel vm) => new()
    {
        CompanyId = vm.CompanyId,
        Year = vm.Year,
        Status = vm.Status,
        EsrsVersion = vm.EsrsVersion,
        MaterialityCompleted = vm.MaterialityCompleted,
        AssuranceLevel = vm.AssuranceLevel
    };

    public static ReportViewModel ToViewModel(this Report report) => new(
        report.Id,
        report.CompanyId,
        report.Year,
        report.Status,
        report.EsrsVersion,
        report.MaterialityCompleted,
        report.AssuranceLevel);
}
