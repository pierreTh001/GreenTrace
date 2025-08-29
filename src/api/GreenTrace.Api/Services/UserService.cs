namespace GreenTrace.Api.Services;

using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Json;

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
        await EnsureRoleExists("Admin");

        var roleUser = await _db.Roles.FirstAsync(r => r.Code == "User");
        _db.UserSystemRoles.Add(new UserSystemRole
        {
            UserId = user.Id,
            RoleId = roleUser.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            CreatedBy = user.Id,
            UpdatedBy = user.Id
        });
        await _db.SaveChangesAsync();

        // If there is no Admin yet, promote first registered user to Admin
        var hasAdmin = await (from usr in _db.UserSystemRoles
                              join r in _db.Roles on usr.RoleId equals r.Id
                              where r.Code == "Admin"
                              select usr).AnyAsync();
        if (!hasAdmin)
        {
            var roleAdmin = await _db.Roles.FirstAsync(r => r.Code == "Admin");
            _db.UserSystemRoles.Add(new UserSystemRole
            {
                UserId = user.Id,
                RoleId = roleAdmin.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = user.Id,
                UpdatedBy = user.Id
            });
            await _db.SaveChangesAsync();
        }

        var rolesForUser = await (from usr in _db.UserSystemRoles
                                  join r in _db.Roles on usr.RoleId equals r.Id
                                  where usr.UserId == user.Id
                                  select r.Code).ToListAsync();
        return (user, rolesForUser);
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

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _db.Users.FindAsync(id).AsTask();
    }

    public async Task<User> UpdateAsync(Guid id, string firstName, string lastName)
    {
        var user = await _db.Users.FindAsync(id) ?? throw new KeyNotFoundException();
        user.FirstName = firstName;
        user.LastName = lastName;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return;
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id) ?? throw new KeyNotFoundException();
        user.IsActive = true;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task UpdatePreferencesAsync(Guid id, object preferences)
    {
        var user = await _db.Users.FindAsync(id) ?? throw new KeyNotFoundException();
        var json = JsonSerializer.Serialize(preferences);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("preferredLocale", out var loc))
        {
            user.PreferredLocale = loc.GetString();
        }
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
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
