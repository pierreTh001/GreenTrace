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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
        modelBuilder.Entity<UserSystemRole>().HasKey(usr => new { usr.UserId, usr.RoleId });
        modelBuilder.Entity<CompanyRelationship>().HasKey(cr => new { cr.ParentCompanyId, cr.ChildCompanyId });
        modelBuilder.Entity<UserCompanyRole>().HasKey(ucr => new { ucr.UserId, ucr.CompanyId, ucr.RoleId });
    }
}
