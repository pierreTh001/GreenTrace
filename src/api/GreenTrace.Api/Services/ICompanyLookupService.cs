using GreenTrace.Api.ViewModels.Companies;

namespace GreenTrace.Api.Services;

public interface ICompanyLookupService
{
    Task<IEnumerable<CompanyLookupItemViewModel>> SearchAsync(string query);
}

