namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantAdminSport:BaseEntity
    {
        public int TenantAdminId { get; set; }
        public TenantAdmin TenantAdmin { get; set; }
        public int SportId { get; set; }
    }
}
