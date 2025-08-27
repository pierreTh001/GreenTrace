using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.Sites;

namespace GreenTrace.Api.Mappers;

public static class SiteMapper
{
    public static CompanySite ToEntity(this CreateSiteViewModel vm, Guid companyId) => new()
    {
        CompanyId = companyId,
        Name = vm.Name,
        AddressLine1 = vm.AddressLine1,
        City = vm.City,
        PostalCode = vm.PostalCode,
        Country = vm.Country,
        GridRegionCode = vm.GridRegionCode,
        Latitude = vm.Latitude,
        Longitude = vm.Longitude
    };

    public static SiteViewModel ToViewModel(this CompanySite site) => new(
        site.Id,
        site.CompanyId,
        site.Name,
        site.AddressLine1,
        site.City,
        site.PostalCode,
        site.Country,
        site.GridRegionCode,
        site.Latitude,
        site.Longitude);
}
