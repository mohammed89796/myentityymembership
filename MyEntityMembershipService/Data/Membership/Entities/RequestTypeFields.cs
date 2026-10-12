namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class RequestTypeFields : SharedEntity
    {
        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsEditable { get; set; }
        public bool? IsChecked { get; set; }
        public int FieldCategoryId { get; set; }
        public FieldCategory FieldCategory { get; set; }
    }
}
