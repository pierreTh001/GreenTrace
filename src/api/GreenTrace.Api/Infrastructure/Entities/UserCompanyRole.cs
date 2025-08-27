namespace GreenTrace.Api.Infrastructure.Entities;

public class UserCompanyRole
{
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid RoleId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}
