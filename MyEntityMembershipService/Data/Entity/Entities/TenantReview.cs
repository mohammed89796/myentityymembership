namespace MyEntityMembershipService.Data;

public class TenantReview : BaseEntity
{
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    
    public int Rating { get; set; }

    public string? Comment { get; set; }

}