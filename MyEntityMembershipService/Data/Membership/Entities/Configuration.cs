namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Configuration:BaseEntity
    {
        public string Name { get; set; }

        public int RequestTypeId { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual ICollection<PackageConfigurationSection> PackageConfigurationSections { get; set; } =new List<PackageConfigurationSection>();
        public virtual ICollection<ConfigrationFieldResponse> ConfigrationFieldResponses { get; set; } = new HashSet<ConfigrationFieldResponse>();

    }
}
