namespace MyEntityMembershipService.Data;

public class Album : BaseEntity
{
    public string Title { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
    public ICollection<Gallery> Galleries { get; set; }
        
}