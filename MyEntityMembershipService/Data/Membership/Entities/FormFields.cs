namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FormFields :SharedEntity
    {
        public int FormId { get; set; }
        public Form Form {  get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsEditable { get; set; }
        //TODO:Index For ReOrder
        public int Index { get; set; }
        public bool ? IsHidden { get; set; }
        
    }
}
