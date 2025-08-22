using GreenTrace.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
}
