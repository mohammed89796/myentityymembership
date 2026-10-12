namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantAdminRole : BaseEntity
    {
        public string? RoleId { get; set; }
        public int TenantAdminId { get; set; }
        public TenantAdmin TenantAdmin { get; set; }    

    }
}
