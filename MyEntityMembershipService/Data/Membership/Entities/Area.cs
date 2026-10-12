namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Area : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
        public ICollection<AreaConfiguration> AreaConfigurations { get; set; } = new HashSet<AreaConfiguration>();
        public ICollection<AreaPricing> AreaPrices { get; set; } = new HashSet<AreaPricing>();
        public ICollection<Tenant> Entities { get; set; } = new HashSet<Tenant>();

        public ICollection<TenantAdminArea> TenantAdminAreas { get; set; } = new HashSet<TenantAdminArea>();
        public ICollection<TenantArea> TenantAreas { get; set; } = new HashSet<TenantArea>();

    }
}
