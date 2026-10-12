namespace MyEntityMembershipService.Data;

public class TermsOfService : BaseEntity
{
    public string Title { get; set; }
    public string? Terms { get; set; }
    public string? TermsUrl { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
}