namespace GreenTrace.Api.Services;

using GreenTrace.Api.Infrastructure.Entities;

public interface IUserService
{
    Task<(User user, IEnumerable<string> roles)> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<(User user, IEnumerable<string> roles)?> LoginAsync(string email, string password);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User> UpdateAsync(Guid id, string firstName, string lastName);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task UpdatePreferencesAsync(Guid id, object preferences);
    Task<bool> ChangePasswordAsync(Guid id, string oldPassword, string newPassword);
    Task MarkForDeletionAsync(Guid id, DateTimeOffset when);
    Task ReactivateAsync(Guid id);
}
