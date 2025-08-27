using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Services;

public class CompanyService : ICompanyService
{
    private readonly AppDbContext _db;

    public CompanyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
        => await _db.Companies.ToListAsync();

    public Task<Company?> GetByIdAsync(Guid id)
        => _db.Companies.FindAsync(id).AsTask();

    public async Task<Company> CreateAsync(Company company)
    {
        if (company.Id == Guid.Empty) company.Id = Guid.NewGuid();
        company.CreatedAt = DateTimeOffset.UtcNow;
        company.UpdatedAt = company.CreatedAt;
        _db.Companies.Add(company);
        await _db.SaveChangesAsync();
        return company;
    }

    public async Task<Company> UpdateAsync(Guid id, Company company)
    {
        var existing = await _db.Companies.FindAsync(id) ?? throw new KeyNotFoundException();
        existing.Name = company.Name;
        existing.LegalForm = company.LegalForm;
        existing.Siren = company.Siren;
        existing.Siret = company.Siret;
        existing.VatNumber = company.VatNumber;
        existing.NaceCode = company.NaceCode;
        existing.Website = company.Website;
        existing.HqCountry = company.HqCountry;
        existing.EmployeesCount = company.EmployeesCount;
        existing.ConsolidationMethod = company.ConsolidationMethod;
        existing.EsgContactEmail = company.EsgContactEmail;
        existing.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null) return;
        _db.Companies.Remove(company);
        await _db.SaveChangesAsync();
    }

    public async Task SetParentAsync(Guid companyId, Guid parentCompanyId)
    {
        var rel = await _db.CompanyRelationships.FindAsync(parentCompanyId, companyId);
        if (rel == null)
        {
            rel = new CompanyRelationship
            {
                ParentCompanyId = parentCompanyId,
                ChildCompanyId = companyId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty
            };
            _db.CompanyRelationships.Add(rel);
        }
        else
        {
            rel.UpdatedAt = DateTimeOffset.UtcNow;
        }
        await _db.SaveChangesAsync();
    }
}
