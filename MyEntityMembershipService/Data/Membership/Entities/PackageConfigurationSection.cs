namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PackageConfigurationSection : BaseEntity
    {
        public string? Name { get; set; }
        public int ConfigurationId { get; set; }
        public virtual Configuration Configuration { get; set; }
        public virtual ICollection<ConfigurationField> ConfigurationFields { get; set; }
    }
}
