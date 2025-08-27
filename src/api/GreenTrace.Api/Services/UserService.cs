namespace GreenTrace.Api.Services;

using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(User user, IEnumerable<string> roles)> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            throw new InvalidOperationException("Email already registered");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await EnsureRoleExists("User");
        var role = await _db.Roles.FirstAsync(r => r.Code == "User");
        _db.UserSystemRoles.Add(new UserSystemRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            CreatedBy = user.Id,
            UpdatedBy = user.Id
        });
        await _db.SaveChangesAsync();

        return (user, new[] { "User" });
    }

    public async Task<(User user, IEnumerable<string> roles)?> LoginAsync(string email, string password)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        var roles = await (from usr in _db.UserSystemRoles
                           join r in _db.Roles on usr.RoleId equals r.Id
                           where usr.UserId == user.Id
                           select r.Code).ToListAsync();

        return (user, roles);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _db.Users.ToListAsync();
    }

    private async Task EnsureRoleExists(string code)
    {
        if (!await _db.Roles.AnyAsync(r => r.Code == code))
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Code = code,
                Label = code,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty
            };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
        }
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf.GetBytes(32);
        var result = new byte[48];
        Buffer.BlockCopy(salt, 0, result, 0, 16);
        Buffer.BlockCopy(hash, 0, result, 16, 32);
        return Convert.ToBase64String(result);
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var bytes = Convert.FromBase64String(stored);
        var salt = new byte[16];
        Buffer.BlockCopy(bytes, 0, salt, 0, 16);
        var storedHash = new byte[32];
        Buffer.BlockCopy(bytes, 16, storedHash, 0, 32);
        var pbkdf = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }
}
