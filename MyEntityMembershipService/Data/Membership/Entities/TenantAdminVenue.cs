namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantAdminVenue : BaseEntity
    {
        public int TenantAdminId { get; set; }
        public TenantAdmin TenantAdmin { get; set; }
        public int VenueId { get; set; }
    }
}
