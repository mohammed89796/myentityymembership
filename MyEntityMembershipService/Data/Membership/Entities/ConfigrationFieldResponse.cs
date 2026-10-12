namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class ConfigrationFieldResponse : SharedEntity
    {
        public int PackageId { get; set; }
        public int ConfigurationId { get; set; }
        public virtual Configuration? Configuration { get; set; }
        public int FieldId { get; set; }
        public virtual ConfigurationField? ConfigurationField { get; set; }
        public string FieldResponse { get; set; }
    }
}
