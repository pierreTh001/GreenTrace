using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.Suppliers;

namespace GreenTrace.Api.Mappers;

public static class SupplierMapper
{
    public static Supplier ToEntity(this CreateSupplierViewModel vm) => new()
    {
        CompanyId = vm.CompanyId,
        Name = vm.Name,
        VatNumber = vm.VatNumber,
        Country = vm.Country
    };

    public static SupplierViewModel ToViewModel(this Supplier supplier) => new(
        supplier.Id,
        supplier.CompanyId,
        supplier.Name,
        supplier.VatNumber,
        supplier.Country);
}
