namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FedrationBranch : BaseEntity
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
    }
}
