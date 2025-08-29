using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenTrace.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesPermissionsPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Subscription plans
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM SubscriptionPlans WHERE Code = 'BASIC')
INSERT INTO SubscriptionPlans (Id, Code, Name, PriceCents, Currency, Interval, IsActive)
VALUES (NEWID(), 'BASIC', 'Basic', 4900, 'EUR', 'month', 1);
IF NOT EXISTS (SELECT 1 FROM SubscriptionPlans WHERE Code = 'PRO')
INSERT INTO SubscriptionPlans (Id, Code, Name, PriceCents, Currency, Interval, IsActive)
VALUES (NEWID(), 'PRO', 'Pro', 9900, 'EUR', 'month', 1);

-- Roles
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Code = 'Admin')
INSERT INTO Roles (Id, Code, Label, Description, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, DeletedAt, Rv)
VALUES (NEWID(), 'Admin', 'Administrator', 'Full system access', SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000', NULL, 0x);
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Code = 'User')
INSERT INTO Roles (Id, Code, Label, Description, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, DeletedAt, Rv)
VALUES (NEWID(), 'User', 'User', 'Standard user', SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000', NULL, 0x);
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Code = 'CompanyOwner')
INSERT INTO Roles (Id, Code, Label, Description, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, DeletedAt, Rv)
VALUES (NEWID(), 'CompanyOwner', 'Company Owner', 'Owner of a company', SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000', NULL, 0x);

-- Permissions (insert if missing by Code)
DECLARE @perm TABLE(Code nvarchar(200));
INSERT INTO @perm (Code) VALUES
('Companies.Read'),('Companies.Write'),('Sites.Read'),('Sites.Write'),('Energy.Read'),('Energy.Write'),('Suppliers.Read'),('Suppliers.Write'),('Documents.Read'),('Documents.Write'),('Reports.Read'),('Reports.Write'),('Emissions.Calculate'),('Factors.Search');

INSERT INTO Permissions (Id, Code, Label, Description, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, DeletedAt, Rv)
SELECT NEWID(), p.Code, p.Code, NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000', NULL, 0x
FROM @perm p
WHERE NOT EXISTS (SELECT 1 FROM Permissions x WHERE x.Code = p.Code);

-- RolePermissions mapping helper: insert missing pairs
DECLARE @AllPerms TABLE(Code nvarchar(200));
INSERT INTO @AllPerms SELECT Code FROM Permissions;

-- Admin gets all listed perms
INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
SELECT r.Id, p.Id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'
FROM Roles r
JOIN Permissions p ON p.Code IN ('Companies.Read','Companies.Write','Sites.Read','Sites.Write','Energy.Read','Energy.Write','Suppliers.Read','Suppliers.Write','Documents.Read','Documents.Write','Reports.Read','Reports.Write','Emissions.Calculate','Factors.Search')
WHERE r.Code = 'Admin'
AND NOT EXISTS (SELECT 1 FROM RolePermissions rp WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id);

-- CompanyOwner gets same set
INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
SELECT r.Id, p.Id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'
FROM Roles r
JOIN Permissions p ON p.Code IN ('Companies.Read','Companies.Write','Sites.Read','Sites.Write','Energy.Read','Energy.Write','Suppliers.Read','Suppliers.Write','Documents.Read','Documents.Write','Reports.Read','Reports.Write','Emissions.Calculate','Factors.Search')
WHERE r.Code = 'CompanyOwner'
AND NOT EXISTS (SELECT 1 FROM RolePermissions rp WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id);

-- User gets read-only subset + factor search
INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
SELECT r.Id, p.Id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'
FROM Roles r
JOIN Permissions p ON p.Code IN ('Companies.Read','Sites.Read','Energy.Read','Suppliers.Read','Documents.Read','Reports.Read','Factors.Search')
WHERE r.Code = 'User'
AND NOT EXISTS (SELECT 1 FROM RolePermissions rp WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id);

");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE rp FROM RolePermissions rp WHERE rp.RoleId IN (SELECT Id FROM Roles WHERE Code IN ('Admin','User','CompanyOwner'));
DELETE FROM Permissions WHERE Code IN ('Companies.Read','Companies.Write','Sites.Read','Sites.Write','Energy.Read','Energy.Write','Suppliers.Read','Suppliers.Write','Documents.Read','Documents.Write','Reports.Read','Reports.Write','Emissions.Calculate','Factors.Search');
DELETE FROM Roles WHERE Code IN ('Admin','User','CompanyOwner');
DELETE FROM SubscriptionPlans WHERE Code IN ('BASIC','PRO');
");
        }
    }
}
