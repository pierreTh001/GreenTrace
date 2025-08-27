namespace GreenTrace.Api.Infrastructure.Entities;

public class EmployeeCommute
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Mode { get; set; } = null!;
    public decimal AvgDistanceKm { get; set; }
    public int EmployeesCount { get; set; }
    public int Days { get; set; }
    public int PeriodYear { get; set; }
    public Guid? EmissionFactorId { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
