namespace GreenTrace.Api.ViewModels.Suppliers;

public record SupplierViewModel(
    Guid Id,
    Guid CompanyId,
    string Name,
    string? VatNumber,
    string? Country);
