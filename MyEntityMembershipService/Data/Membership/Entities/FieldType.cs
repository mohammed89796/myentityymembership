namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FieldType : BaseEntity
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }
        public List<ConfigurationField> ConfigurationFields { get; set; }

    }
}
