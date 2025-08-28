using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSystemRole> UserSystemRoles => Set<UserSystemRole>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyRelationship> CompanyRelationships => Set<CompanyRelationship>();
    public DbSet<CompanySite> CompanySites => Set<CompanySite>();
    public DbSet<UserCompanyRole> UserCompanyRoles => Set<UserCompanyRole>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportStatusHistory> ReportStatusHistories => Set<ReportStatusHistory>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<StakeholderGroup> StakeholderGroups => Set<StakeholderGroup>();
    public DbSet<MaterialityAssessment> MaterialityAssessments => Set<MaterialityAssessment>();
    public DbSet<MaterialTopic> MaterialTopics => Set<MaterialTopic>();
    public DbSet<Target> Targets => Set<Target>();
    public DbSet<Kpi> Kpis => Set<Kpi>();
    public DbSet<KpiValue> KpiValues => Set<KpiValue>();
    public DbSet<EmissionFactor> EmissionFactors => Set<EmissionFactor>();
    public DbSet<ActivityData> ActivityData => Set<ActivityData>();
    public DbSet<EmissionResult> EmissionResults => Set<EmissionResult>();
    public DbSet<ElectricityContract> ElectricityContracts => Set<ElectricityContract>();
    public DbSet<EnergyEntry> EnergyEntries => Set<EnergyEntry>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchasedGoodsLine> PurchasedGoodsLines => Set<PurchasedGoodsLine>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<BusinessTrip> BusinessTrips => Set<BusinessTrip>();
    public DbSet<EmployeeCommute> EmployeeCommutes => Set<EmployeeCommute>();
    public DbSet<WasteEntry> WasteEntries => Set<WasteEntry>();
    public DbSet<WaterEntry> WaterEntries => Set<WaterEntry>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Global default for decimals
        configurationBuilder.Properties<decimal>().HavePrecision(18, 6);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
        modelBuilder.Entity<UserSystemRole>().HasKey(usr => new { usr.UserId, usr.RoleId });
        modelBuilder.Entity<CompanyRelationship>().HasKey(cr => new { cr.ParentCompanyId, cr.ChildCompanyId });
        modelBuilder.Entity<UserCompanyRole>().HasKey(ucr => new { ucr.UserId, ucr.CompanyId, ucr.RoleId });
        modelBuilder.Entity<SubscriptionPlan>().HasIndex(p => p.Code).IsUnique();
        modelBuilder.Entity<Subscription>().HasIndex(s => new { s.UserId, s.Status });

        // Explicit decimal precision to avoid truncation warnings
        modelBuilder.Entity<WaterEntry>()
            .Property(w => w.VolumeM3)
            .HasPrecision(18, 2);

        // Monetary
        modelBuilder.Entity<PurchasedGoodsLine>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        // Lat/Long
        modelBuilder.Entity<CompanySite>()
            .Property(s => s.Latitude)
            .HasPrecision(9, 6);
        modelBuilder.Entity<CompanySite>()
            .Property(s => s.Longitude)
            .HasPrecision(9, 6);

        // Percentages
        modelBuilder.Entity<EnergyEntry>()
            .Property(e => e.RenewableSharePct)
            .HasPrecision(5, 2);
        modelBuilder.Entity<CompanyRelationship>()
            .Property(c => c.EquitySharePct)
            .HasPrecision(5, 2);

        // Emission factor high precision
        modelBuilder.Entity<EmissionFactor>()
            .Property(f => f.FactorCo2e)
            .HasPrecision(18, 8);

        // Distances / masses
        modelBuilder.Entity<EnergyEntry>()
            .Property(e => e.ConsumptionKwh)
            .HasPrecision(18, 3);
        modelBuilder.Entity<Shipment>()
            .Property(s => s.DistanceTkm)
            .HasPrecision(18, 3);
        modelBuilder.Entity<BusinessTrip>()
            .Property(b => b.DistanceKm)
            .HasPrecision(18, 3);
        modelBuilder.Entity<WasteEntry>()
            .Property(w => w.MassTons)
            .HasPrecision(18, 3);
        modelBuilder.Entity<EmployeeCommute>()
            .Property(e => e.AvgDistanceKm)
            .HasPrecision(18, 3);
    }
}
