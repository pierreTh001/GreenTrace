using GreenTrace.Api.Infrastructure;
using Entities = GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1) Connexion SQL (en Azure: ConnectionStrings__Default via Key Vault reference)
var connectionString = builder.Configuration.GetConnectionString("Default");

// 2) EF Core
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// 3) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    // Permet d'utiliser [SwaggerOperation] pour les résumés/descriptions
    o.EnableAnnotations();
});

// Authentication & Authorization
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Subscribed", policy => policy.Requirements.Add(new GreenTrace.Api.Authorization.SubscribedRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, GreenTrace.Api.Authorization.SubscribedHandler>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IEmissionService, EmissionService>();
builder.Services.AddScoped<IEmissionFactorService, EmissionFactorService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEnergyService, EnergyService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IActivityDataService, ActivityDataService>();
builder.Services.AddScoped<IRbacService, RbacService>();

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

app.UseAuthentication();
app.UseAuthorization();

// 6) Endpoints
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { ok = true, at = DateTime.UtcNow }));

app.MapGet("/companies", async (AppDbContext db) =>
{
    var all = await db.Companies.OrderBy(c => c.Id).ToListAsync();
    return Results.Ok(all);
});

app.MapPost("/companies", async (AppDbContext db, CompanyDto dto) =>
{
    var c = new Entities.Company
    {
        Id = Guid.NewGuid(),
        Name = dto.LegalName,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
        CreatedBy = Guid.Empty,
        UpdatedBy = Guid.Empty
    };
    db.Companies.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/companies/{c.Id}", c);
});

app.Run();

// DTO local (léger) — si tu préfères, mets-le dans un fichier séparé.
public record CompanyDto(string LegalName);
