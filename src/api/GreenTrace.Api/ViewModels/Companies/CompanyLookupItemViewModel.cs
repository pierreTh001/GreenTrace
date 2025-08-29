namespace GreenTrace.Api.ViewModels.Companies;

public record CompanyLookupItemViewModel(
    string Name,
    string? LegalForm,
    string Siren,
    string? Siret,
    string? VatNumber,
    string? NaceCode,
    string? AddressLine1,
    string? PostalCode,
    string? City,
    string? Country);

