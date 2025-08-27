namespace GreenTrace.Api.ViewModels.Sites;

public record SiteViewModel(
    Guid Id,
    Guid CompanyId,
    string Name,
    string? AddressLine1,
    string? City,
    string? PostalCode,
    string? Country,
    string? GridRegionCode,
    decimal? Latitude,
    decimal? Longitude);
