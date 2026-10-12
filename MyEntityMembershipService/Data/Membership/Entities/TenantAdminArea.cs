namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantAdminArea : BaseEntity
    {

        public Area Area { get; set; }
        public int AreaId { get; set; }

        public TenantAdmin TenantAdmin { get; set; }
        public int TenantAdminId { get; set; }
    }
}
