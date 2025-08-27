namespace GreenTrace.Api.Services;

using GreenTrace.Api.Infrastructure.Entities;

public interface IUserService
{
    Task<(User user, IEnumerable<string> roles)> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<(User user, IEnumerable<string> roles)?> LoginAsync(string email, string password);
    Task<IEnumerable<User>> GetAllAsync();
}
