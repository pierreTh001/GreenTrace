using GreenTrace.Api.ViewModels.Companies;
using GreenTrace.Api.ViewModels.Reports;

namespace GreenTrace.Api.ViewModels.Users;

public record UserCompanyInfoViewModel(
    CompanyViewModel Company,
    IEnumerable<string> Roles,
    IEnumerable<ReportViewModel> Reports);

public record UserDetailViewModel(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IEnumerable<string> Roles,
    IEnumerable<UserCompanyInfoViewModel> Companies);

