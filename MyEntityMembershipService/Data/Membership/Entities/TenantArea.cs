namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantArea : BaseEntity
    {
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public int AreaId { get; set; }
        public Area Area { get; set; }
    }
}
