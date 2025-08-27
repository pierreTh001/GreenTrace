using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.Companies;

namespace GreenTrace.Api.Mappers;

public static class CompanyMapper
{
    public static Company ToEntity(this CreateCompanyViewModel vm) => new()
    {
        Name = vm.Name,
        LegalForm = vm.LegalForm,
        Siren = vm.Siren,
        Siret = vm.Siret,
        VatNumber = vm.VatNumber,
        NaceCode = vm.NaceCode,
        Website = vm.Website,
        HqCountry = vm.HqCountry,
        EmployeesCount = vm.EmployeesCount,
        ConsolidationMethod = vm.ConsolidationMethod,
        EsgContactEmail = vm.EsgContactEmail
    };

    public static void MapTo(this UpdateCompanyViewModel vm, Company company)
    {
        company.Name = vm.Name;
        company.LegalForm = vm.LegalForm;
        company.Siren = vm.Siren;
        company.Siret = vm.Siret;
        company.VatNumber = vm.VatNumber;
        company.NaceCode = vm.NaceCode;
        company.Website = vm.Website;
        company.HqCountry = vm.HqCountry;
        company.EmployeesCount = vm.EmployeesCount;
        company.ConsolidationMethod = vm.ConsolidationMethod;
        company.EsgContactEmail = vm.EsgContactEmail;
    }

    public static CompanyViewModel ToViewModel(this Company company) => new(
        company.Id,
        company.Name,
        company.LegalForm,
        company.Siren,
        company.Siret,
        company.VatNumber,
        company.NaceCode,
        company.Website,
        company.HqCountry,
        company.EmployeesCount,
        company.ConsolidationMethod,
        company.EsgContactEmail);
}
