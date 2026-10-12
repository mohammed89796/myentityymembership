namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class ConfigurationField:BaseEntity
    {
        public string Name { get; set; }
        public string? FieldUnit { get; set; }
        public int FieldTypeId { get; set; }
        public FieldType FieldType { get; set; }

        public int PackageConfigurationSectionId { get; set; }
        public virtual PackageConfigurationSection PackageConfigurationSection { get; set; }

        public virtual ICollection<DropdownValue> DropdownValues { get; set; } = new List<DropdownValue>();
        public virtual ICollection<ConfigrationFieldResponse> ConfigrationFieldResponses { get; set; } = new HashSet<ConfigrationFieldResponse>();

    }
}
