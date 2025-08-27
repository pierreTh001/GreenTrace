namespace GreenTrace.Api.ViewModels.Suppliers;

public record CreateSupplierViewModel(
    Guid CompanyId,
    string Name,
    string? VatNumber,
    string? Country);
