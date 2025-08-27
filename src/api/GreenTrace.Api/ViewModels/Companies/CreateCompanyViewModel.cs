namespace GreenTrace.Api.ViewModels.Companies;

public record CreateCompanyViewModel(
    string Name,
    string? LegalForm,
    string? Siren,
    string? Siret,
    string? VatNumber,
    string? NaceCode,
    string? Website,
    string? HqCountry,
    int? EmployeesCount,
    string? ConsolidationMethod,
    string? EsgContactEmail);
