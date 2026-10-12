namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FedrationEntityType : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
    }
}