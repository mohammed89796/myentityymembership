namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FieldResponse: BaseEntity
    {
        public string Response {  get; set; }
        public int FormUserResponseId { get; set; }
        public FormUserResponse FormUserResponse { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public int? UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}
