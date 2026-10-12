namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TenantAdminBranch: BaseEntity
    {
        public int TenantAdminId { get; set; }
        public TenantAdmin TenantAdmin { get; set; }
        public int BranchId { get; set; }
    }
}
