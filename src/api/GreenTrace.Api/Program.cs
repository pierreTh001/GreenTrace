using GreenTrace.Api.Domain;
using GreenTrace.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Connexion SQL (en Azure: ConnectionStrings__Default via Key Vault reference)
var connectionString = builder.Configuration.GetConnectionString("Default");

// 2) EF Core
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// 3) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4) Build (⚠️ c’est bien Build(), pas Create(...))
var app = builder.Build();

// 5) Dev-only: Swagger + migrations auto (optionnel)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 6) Endpoints
app.MapGet("/health", () => Results.Ok(new { ok = true, at = DateTime.UtcNow }));

app.MapGet("/companies", async (AppDbContext db) =>
{
    var all = await db.Companies.OrderBy(c => c.Id).ToListAsync();
    return Results.Ok(all);
});

app.MapPost("/companies", async (AppDbContext db, CompanyDto dto) =>
{
    var c = new Company { LegalName = dto.LegalName };
    db.Companies.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/companies/{c.Id}", c);
});

app.Run();

// DTO local (léger) — si tu préfères, mets-le dans un fichier séparé.
public record CompanyDto(string LegalName);
