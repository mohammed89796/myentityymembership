namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantAdminFacility : BaseEntity
    {
        public int TenantAdminId { get; set; }
        public TenantAdmin TenantAdmin { get; set; }
        public int FacilityId { get; set; }
    }
}
