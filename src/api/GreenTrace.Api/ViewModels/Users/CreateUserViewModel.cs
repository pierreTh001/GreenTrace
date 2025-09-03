namespace GreenTrace.Api.ViewModels.Users;

public record CreateUserViewModel(string Email, string Password, string FirstName, string LastName, string? Role = null);
