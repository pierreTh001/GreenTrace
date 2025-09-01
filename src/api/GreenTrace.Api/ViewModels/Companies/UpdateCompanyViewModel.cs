using System.ComponentModel.DataAnnotations;

namespace GreenTrace.Api.ViewModels.Companies;

public record UpdateCompanyViewModel(
    [Required, MinLength(2), MaxLength(200)] string Name,
    [Required] string LegalForm,
    [Required, RegularExpression(@"^\d{9}$", ErrorMessage = "SIREN invalide (9 chiffres)")] string Siren,
    [Required, RegularExpression(@"^\d{14}$", ErrorMessage = "SIRET invalide (14 chiffres)")] string Siret,
    [Required] string VatNumber,
    [Required] string NaceCode,
    [Required, RegularExpression(@"^(https?://)?([A-Za-z0-9\-]+\.)+[A-Za-z]{2,}$", ErrorMessage = "Website invalide (ex: exemple.fr ou https://exemple.fr)")] string Website,
    [Required, MaxLength(255)] string AddressLine1,
    [MaxLength(255)] string? AddressLine2,
    [Required, RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9 \-]{1,10}$", ErrorMessage = "Code postal invalide")] string PostalCode,
    [Required, RegularExpression(@"^[^\d]+$", ErrorMessage = "La ville ne doit pas contenir de chiffres")] string City,
    [Required, MaxLength(100)] string HqCountry,
    [Required, Range(0, int.MaxValue)] int EmployeesCount,
    [Required] string ConsolidationMethod,
    [Required, EmailAddress] string EsgContactEmail);
