namespace GreenTrace.Api.ViewModels.Sites;

public record CreateSiteViewModel(
    string Name,
    string? AddressLine1,
    string? City,
    string? PostalCode,
    string? Country,
    string? GridRegionCode,
    decimal? Latitude,
    decimal? Longitude);
