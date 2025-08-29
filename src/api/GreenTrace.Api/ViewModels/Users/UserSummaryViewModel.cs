namespace GreenTrace.Api.ViewModels.Users;

public record UserSummaryViewModel(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IEnumerable<string> Roles);

