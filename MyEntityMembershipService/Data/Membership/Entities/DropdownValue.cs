namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class DropdownValue : BaseEntity
    {
        public string Value { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }

        public int? ConfigurationFieldId { get; set; }
        public ConfigurationField ConfigurationField { get; set; }
    }
}
